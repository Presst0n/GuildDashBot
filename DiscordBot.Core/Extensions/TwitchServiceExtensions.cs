using Discord;
using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;

namespace DiscordBot.Core.Extensions
{
    public static class TwitchServiceExtensions
    {
        public static async Task<StreamerDbModel> MapStreamers(this StreamerDbModel _streamer, TwitchAPI api)
        {
            List<string> gameIds = new List<string>();
            List<string> streamerIds = new List<string>();
            List<string> logins = new List<string>
            {
                _streamer.StreamerLogin
            };

            var streamersOnline = await api.Helix.Streams.GetStreamsAsync(null, null, 20, null, null, "all", null, logins);

            gameIds.Add(streamersOnline.Streams[0].GameId);
            streamerIds.Add(streamersOnline.Streams[0].UserId);

            var game = await api.Helix.Games.GetGamesAsync(gameIds, null);
            var followers = await api.Helix.Users.GetUsersFollowsAsync(null, null, 20, null, streamersOnline.Streams[0].UserId);
            var streamerData = await api.Helix.Users.GetUsersAsync(streamerIds, null, null);

            if (game.Games.Length != 0)
            {
                _streamer.PlayedGame = game.Games[0].Name;
            }
            else
            {
                _streamer.PlayedGame = "brak danych";
            }

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
            _streamer.TotalFollows = followers.TotalFollows;
            _streamer.Viewers = streamersOnline.Streams[0].ViewerCount;

            return _streamer;
        }

        public static Embed GetEmbededMessage(this StreamerDbModel mappedStreamer)
        {
            if (string.IsNullOrEmpty(mappedStreamer.StreamerLogin))
            {
                throw new ArgumentException("Error! Streamer's name is null or empty.", "StreamerLogin");
            }

            try
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle($"{mappedStreamer.StreamerLogin} właśnie streamuje!");
                builder.AddField("Tytuł:", $"{mappedStreamer.StreamTitle}", false);
                builder.AddField("Widzowie:", $"{mappedStreamer.Viewers}", false);
                builder.AddField("Gra:", $"{mappedStreamer.PlayedGame}", false);
                builder.AddField("Obserwujący:", $"{mappedStreamer.TotalFollows}");
                builder.WithThumbnailUrl($"{mappedStreamer.ProfileImage}");
                builder.WithUrl($"{mappedStreamer.UrlAddress}");
                builder.WithColor(Color.Blue);

                var output = builder.Build();
                return output;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"{ex.Message}");
                throw ex;
            }
        }
    }
}
