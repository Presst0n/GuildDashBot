using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Config;
using DiscordBot.Core.Data.DataAccess;
using DiscordBot.Core.Extensions;
using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;

namespace DiscordBot.Core.Services
{
    public class TwitchService
    {
        private static TwitchAPI api;
        private readonly DiscordSocketClient _client;
        private IEnumerable<StreamerDbModel> _streamersFromDb;
        private IDataAccess _database;


        public TwitchService(IDataAccess database, DiscordSocketClient client)
        {
            _client = client;
            _database = database;
        }

        public async Task InitializeConnection()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = GuildBotConfig.bot.twitchClient;
            api.Settings.AccessToken = GuildBotConfig.bot.twitchToken;

            await ResetStreamer();
            MonitorStreams();
        }

        public async Task MonitorStreams()
        {
            while (true)
            {
                try
                {
                    if (IsRunning())
                    {
                        var streamersLoginsFromDb = _database.GetStreamers().Result.Select(x => x.StreamerLogin).ToList();

                        if (streamersLoginsFromDb.Count > 0)
                        {
                            var streams = await api.Helix.Streams.GetStreamsAsync(null, null, 20, null, null, "all", null, streamersLoginsFromDb);

                            if (await CheckIfStreaming(streams.Streams))
                            {
                                await ProcessingStreams(streams.Streams);
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(3));
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine($"{ex.Source}, {ex.InnerException}, {ex.Message}");
                    throw new NullReferenceException($"{ex.StackTrace}, {ex.InnerException}, {ex.Message}, {ex.Source}");
                }
            }
        }

        public bool IsRunning() =>
            _client.ConnectionState == ConnectionState.Connected ||
            _client.ConnectionState == ConnectionState.Connecting;

        public async Task ResetStreamer()
        {
            var loadedStreamers = await _database.GetStreamers();
            foreach (var streamer in loadedStreamers)
            {
                streamer.SentMessage = false;
                streamer.IsStreaming = false;
                //streamer.PlayedGame = "";
                //streamer.ProfileImage = "";
                //streamer.StreamTitle = "";
                streamer.TotalFollows = 0;
                streamer.Viewers = 0;

                await _database.AddStreamerAsync(streamer);
            }
        }

        public async Task<bool> CheckIfStreaming(Stream[] streamingStreamers)
        {
            bool output = true;
            var streamersFromDb = await _database.GetStreamers();

            if (streamingStreamers.Length < 1)
            {
                foreach (var streamer in streamersFromDb)
                {
                    streamer.SentMessage = false;
                    streamer.IsStreaming = false;

                    await _database.AddStreamerAsync(streamer);
                }

                output = false;
            }

            _streamersFromDb = streamersFromDb;
            return output;
        }

        public async Task ProcessingStreams(Stream[] streamingStreamers)
        {
            var streamersIds = new HashSet<string>(streamingStreamers.Select(x => x.UserId));
            var streamersOnline = _streamersFromDb.Where(s => streamersIds.Contains(s.StreamerId));

            foreach (var streamer in streamersOnline)
            {
                var mappedStreamer = await streamer.MapStreamers(api);

                if (mappedStreamer.SentMessage == false)
                {
                    if (await NotifyAboutStream(mappedStreamer))
                    {
                        mappedStreamer.SentMessage = true;
                    }
                    else
                    {
                        mappedStreamer.SentMessage = false;
                        mappedStreamer.IsStreaming = true;
                    }

                    await _database.AddStreamerAsync(mappedStreamer);
                }
            }

            var excludedIDs = new HashSet<string>(streamingStreamers.Select(x => x.UserId));
            var result = _streamersFromDb.Where(s => !excludedIDs.Contains(s.StreamerId));

            foreach (var streamer in result)
            {
                streamer.SentMessage = false;
                streamer.IsStreaming = false;
                await _database.AddStreamerAsync(streamer);
            }
        }

        public async Task<bool> NotifyAboutStream(StreamerDbModel mappedStreamer)
        {
            if (!IsRunning()) { return false; }

            bool output = true;

            var model = await _database.GetId();

            if (model is null) { return false; }

            if (model.Channel_Id != 0)
            {
                var chnl = _client.GetChannel(model.Channel_Id) as IMessageChannel;
                await chnl.SendMessageAsync(null, false, mappedStreamer.GetEmbededMessage());
            }
            else
            {
                output = false;
            }

            return output;
        }
    }
}
