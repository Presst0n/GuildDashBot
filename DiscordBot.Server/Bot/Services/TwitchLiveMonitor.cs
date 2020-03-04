using Abstractions;
using Abstractions.Db;
using Discord;
using Discord.WebSocket;
using DiscordBot.LibraryData.Models;
using DiscordBot.Server.Bot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace DiscordBot.Server.Bot.Services
{
    public class TwitchLiveMonitor : ITwitchLiveMonitor
    {
        private LiveStreamMonitorService _monitor;
        private TwitchAPI _api;
        private readonly DiscordSocketClient _client;
        private readonly IGuildBot _bot;
        private readonly IBotDbService _botDbService;

        public TwitchLiveMonitor(DiscordSocketClient client, IGuildBot bot, IBotDbService botDbService)
        {
            _client = client;
            _bot = bot;
            _botDbService = botDbService;
        }

        public Task StartLiveMonitor()
        {
            Task.Run(() => ConfigLiveMonitorAsync());
            return Task.CompletedTask;
        }

        private Task StopLiveMonitor()
        {
            _monitor.Stop();
            _monitor.ClearCache();

            return Task.CompletedTask;
        }

        private async Task ConfigLiveMonitorAsync()
        {
            if (_api == null && _monitor == null)
            {
                _api = new TwitchAPI();
                _api.Settings.ClientId = GuildBotConfig.bot.twitchClient;
                _api.Settings.AccessToken = GuildBotConfig.bot.twitchToken;

                _monitor = new LiveStreamMonitorService(_api, 180, 5);

                List<string> ChannelsToMonitor = new List<string>();

                var listOfStreamers = await _botDbService.GetStreamers();

                if (listOfStreamers != null)
                {
                    listOfStreamers.ToList()
                        .Select(x => x.StreamerLogin).ToList()
                        .ForEach(x => ChannelsToMonitor
                        .Add(x.ToLower()));
                }
                else
                {
                    ChannelsToMonitor.Add("DummyDummyStreamer1337");
                }

                _client.Connected += OnClientConnected;
                _client.Disconnected += OnClientDisconnected;
                _monitor.SetChannelsByName(ChannelsToMonitor);
                _monitor.OnStreamOnline += Monitor_OnStreamOnline;
                _monitor.Start();
            }
            else
            {
                if (!_monitor.Enabled)
                {
                    await CheckNewChannelsToMonitor();
                    _monitor.Start();
                }
            }
        }

        private Task OnClientConnected()
        {
            if (!IsEnabled())
            {
                _monitor.Start();
            }

            return Task.CompletedTask;
        }

        private Task OnClientDisconnected(Exception arg)
        {
            if (IsEnabled())
            {
                StopLiveMonitor();
            }

            return Task.CompletedTask;
        }

        public bool IsEnabled()
        {
            if (_monitor is null)
            {
                return false;
            }
            else
            {
                return _monitor.Enabled;
            }
        }

        public async Task CheckNewChannelsToMonitor()
        {
            List<string> ChannelsToMonitor = new List<string>();
            var listOfStreamers = await _botDbService.GetStreamers();
            if (listOfStreamers is null) { return; }

            listOfStreamers.ToList()
                .Select(x => x.StreamerLogin).ToList()
                .ForEach(x => ChannelsToMonitor
                .Add(x.ToLower()));

            _monitor.SetChannelsByName(ChannelsToMonitor);
        }

        private async void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            if (!_bot.IsRunning())
            {
                await StopLiveMonitor();
                return;
            }

            if (e.Channel.Length != 0)
            {
                var streamer = new StreamerDbModel
                {
                    Viewers = e.Stream.ViewerCount,
                    IsStreaming = true,
                    StreamerLogin = e.Channel
                };

                if (e.Stream.Title.Length != 0)
                {
                    if (e.Stream.Title.Contains("\n"))
                    {
                        var title = e.Stream.Title;
                        title = title.Replace("\n", " ");
                        streamer.StreamTitle = title;
                    }
                    else
                    {
                        streamer.StreamTitle = e.Stream.Title;
                    }
                }
                else
                {
                    streamer.StreamTitle = "Brak tytułu";
                }

                var streamerMapped = await MapStreamerModel(streamer, e.Stream, e.Channel);
                await SendNMotificationAsync(streamerMapped);
            }
        }

        private async Task<StreamerDbModel> MapStreamerModel(StreamerDbModel model, Stream stream, string channelName)
        {
            List<string> gameIds = new List<string>();
            List<string> streamerIds = new List<string>();

            gameIds.Add(stream.GameId);
            streamerIds.Add(stream.UserId);

            var game = await _api.Helix.Games.GetGamesAsync(gameIds, null);
            var followers = await _api.Helix.Users.GetUsersFollowsAsync(null, null, 20, null, stream.UserId);
            var streamerData = await _api.Helix.Users.GetUsersAsync(streamerIds, null, null);

            if (game.Games.Length != 0)
            {
                model.PlayedGame = game.Games[0].Name;
            }
            else
            {
                model.PlayedGame = "brak danych";
            }

            model.ProfileImage = streamerData.Users[0].ProfileImageUrl;
            model.TotalFollows = followers.TotalFollows;
            model.UrlAddress = $"https://www.twitch.tv/{channelName}";
            return model;
        }

        private Task<Embed> GetEmbededData(StreamerDbModel streamer)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{streamer.StreamerLogin} właśnie streamuje!");
            builder.AddField("Tytuł:", $"{streamer.StreamTitle}", false);
            builder.AddField("Widzowie:", $"{streamer.Viewers}", false);
            builder.AddField("Gra:", $"{streamer.PlayedGame}", false);
            builder.AddField("Obserwujący:", $"{streamer.TotalFollows}");
            builder.WithThumbnailUrl($"{streamer.ProfileImage}");
            builder.WithUrl($"{streamer.UrlAddress}");
            builder.WithColor(Color.Blue);

            var output = builder.Build();
            return Task.FromResult(output);
        }

        private async Task SendNMotificationAsync(StreamerDbModel streamer)
        {
            ChannelDbModel model = await _botDbService.GetId();

            if (model is null || model.Channel_Id == 0) { return; }

            var result = await GetEmbededData(streamer);
            var chnl = _client.GetChannel(model.Channel_Id) as IMessageChannel;
            await chnl.SendMessageAsync(null, false, result);
        }
    }
}
