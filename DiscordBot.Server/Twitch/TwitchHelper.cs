using DiscordBot.Core.Config;
using DiscordBot.LibraryData.Models;
using DiscordBot.Server.Resources.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;

namespace DiscordBot.Server.Twitch
{
    public static class TwitchHelper
    {
        private static readonly TwitchAPI api;

        static TwitchHelper()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = GuildBotConfig.bot.twitchClient;
            api.Settings.AccessToken = GuildBotConfig.bot.twitchToken;
        }

        public async static Task<GetUsersResponse> GetStreamer(string login)
        {
            List<string> logins = new List<string>
            {
                login
            };

            return await api.Helix.Users.GetUsersAsync(null, logins, null);
        }

        public static async Task<StreamerDbModel> GetAndMapStreamerDataAsync(this StreamerDbModel _streamer)
        {
            List<string> gameIds = new List<string>();
            List<string> streamerIds = new List<string>();
            List<string> logins = new List<string>
            {
                _streamer.StreamerLogin
            };

            var streamersOnline = await api.Helix.Streams.GetStreamsAsync(null, null, 20, null, null, "all", null, logins);

            if (streamersOnline.Streams.Length != 0)
            {
                gameIds.Add(streamersOnline.Streams[0].GameId);
                streamerIds.Add(streamersOnline.Streams[0].UserId);

                var game = await api.Helix.Games.GetGamesAsync(gameIds, null);
                var streamerData = await api.Helix.Users.GetUsersAsync(streamerIds, null, null);

                if (game.Games.Length != 0)
                {
                    _streamer.PlayedGame = game.Games[0].Name;
                }
                else
                {
                    _streamer.PlayedGame = "brak danych";
                }

                _streamer.StreamerId = streamerData.Users[0].Id;
                _streamer.IsStreaming = true;

                if (streamersOnline.Streams[0].Title.Length != 0)
                {
                    if (streamersOnline.Streams[0].Title.Contains("\n"))
                    {
                        var title = streamersOnline.Streams[0].Title;
                        title = title.Replace("\n", " ");
                        _streamer.StreamTitle = title;
                    }
                    else
                    {
                        _streamer.StreamTitle = streamersOnline.Streams[0].Title;
                    }
                }
                else
                {
                    _streamer.StreamTitle = "Brak tytułu";
                }

                _streamer.ProfileImage = streamerData.Users[0].ProfileImageUrl;
            }
            else
            {
                var result = await api.Helix.Users.GetUsersAsync(null, logins, null);

                if (result.Users.Length != 0)
                {
                    _streamer.ProfileImage = result.Users[0].ProfileImageUrl;
                    _streamer.PlayedGame = "Unknown";
                    _streamer.StreamTitle = "Unknown";
                    _streamer.StreamerId = result.Users[0].Id;
                    _streamer.IsStreaming = false;
                }
                else
                {
                    _streamer.ProfileImage = Images.TwitchDefaultPicture;
                    _streamer.PlayedGame = "Unknown";
                    _streamer.StreamTitle = "Unknown";
                    _streamer.StreamerId = "Unknown";
                    _streamer.IsStreaming = false;
                }
            }

            return _streamer;
        }
    }
}
