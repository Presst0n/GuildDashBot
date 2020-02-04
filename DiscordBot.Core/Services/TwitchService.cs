using Abstractions;
using Abstractions.Db;
using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Config;
using DiscordBot.Core.Extensions;
using DiscordBot.Core.TwitchApi;
using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;

namespace DiscordBot.Core.Services
{
    public class TwitchService
    {
        private readonly TwitchAPI _twitchApi;
        private readonly DiscordSocketClient _client;
        private readonly IDataAccess _database;
        private readonly ILogger _logger;
        private IEnumerable<StreamerDbModel> _loadedStreamers = null;
        private readonly GuildBot _bot;

        public TwitchService(IDataAccess database, DiscordSocketClient client, ILogger logger, GuildBot bot, TwitchApiProvider twitchApi)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _bot = bot;
            _twitchApi = twitchApi.GetApi().Result;
        }

        public async Task InitializeService()
        {
            //Timer t = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds)
            //{
            //    AutoReset = true
            //};
            //t.Elapsed += OnElapsed;
            //t.Start();
            await MonitorStreams();
        }

        //private async void OnElapsed(object sender, ElapsedEventArgs e)
        //{
        //    await MonitorStreams();
        //}

        private async Task MonitorStreams()
        {
            try
            {
                //while (true)
                //{
                //if (_bot.IsRunning())
                //{
                var logins = _database.GetStreamers().Result.Select(x => x.StreamerLogin).ToList();

                if (logins.Count > 0)
                {
                    var response = await _twitchApi.Helix.Streams.GetStreamsAsync(null, null, 20, null, null, "all", null, logins);
                    if (response != null)
                    {
                        if (response.Streams.Length != 0)
                        {
                            _loadedStreamers = await _database.GetStreamers();
                            await ProcessStreams(response.Streams);
                        }
                        else
                        {
                            await ResetStreamers();
                        }
                    }

                }
                // }
                _loadedStreamers = null;
                //GC.Collect();

                // await Task.Delay(TimeSpan.FromMinutes(3));
                //}
            }
            catch (Exception ex)
            {
                _logger.Log($"TwitchService: {ex.Message} -- {ex.StackTrace}");
            }
        }

        public async Task ResetStreamers()
        {
            var loadedStreamers = await _database.GetStreamers();
            foreach (var streamer in loadedStreamers)
            {
                streamer.SentMessage = false;
                streamer.IsStreaming = false;
                streamer.TotalFollows = 0;
                streamer.Viewers = 0;

                await _database.AddStreamerAsync(streamer);
            }
        }

        // TODO - Refactor this method.
        private async Task ProcessStreams(Stream[] streamingStreamers)
        {
            var streamersOnline = await GetStreamersOnline(streamingStreamers);

            foreach (var streamer in streamersOnline)
            {
                var model = await streamer.MapStreamer(_twitchApi);

                if (model.SentMessage == false)
                {
                    var sentSuccessfully = await SendNotification(model);

                    if (sentSuccessfully)
                    {
                        model.SentMessage = true;
                    }
                    else
                    {
                        model.SentMessage = false;
                        model.IsStreaming = true;
                    }

                    await _database.AddStreamerAsync(model);
                }
            }

            var result = await GetStreamersOffline(streamingStreamers);
            await SetOffline(result);
        }

        private async Task SetOffline(IEnumerable<StreamerDbModel> streamers)
        {
            foreach (var streamer in streamers)
            {
                streamer.SentMessage = false;
                streamer.IsStreaming = false;
                await _database.AddStreamerAsync(streamer);
            }
        }

        private Task<IEnumerable<StreamerDbModel>> GetStreamersOnline(Stream[] streamingStreamers)
        {
            var ids = new HashSet<string>(streamingStreamers.Select(x => x.UserId));
            var result = _loadedStreamers.Where(s => ids.Contains(s.StreamerId));

            return Task.FromResult(result);
        }

        private Task<IEnumerable<StreamerDbModel>> GetStreamersOffline(Stream[] streamingStreamers)
        {
            var ids = new HashSet<string>(streamingStreamers.Select(x => x.UserId));
            var result = _loadedStreamers.Where(s => !ids.Contains(s.StreamerId));

            return Task.FromResult(result);
        }

        private async Task<bool> SendNotification(StreamerDbModel streamer)
        {
            if (!_bot.IsRunning()) { return false; }

            bool output = true;
            var channel = await _database.GetId();

            if (channel is null)
            {
                _logger.Log("TwitchService: Cannot send notification because ChannelDbModel is null. " +
                    "For this feature to work, set the channel on which you want notifications to be displayed");

                return false;
            }
            else if (channel.Channel_Id != 0)
            {
                await SendMessageToChannelAsync(streamer, channel);
            }
            else
            {
                output = false;
            }

            return output;
        }

        private async Task SendMessageToChannelAsync(StreamerDbModel streamer, ChannelDbModel model)
        {
            var chnl = _client.GetChannel(model.Channel_Id) as IMessageChannel;
            await chnl.SendMessageAsync(null, false, streamer.GetEmbededData());
        }
    }
}
