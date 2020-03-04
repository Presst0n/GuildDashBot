using DiscordBot.Server.Bot.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;

namespace DiscordBot.Server.Bot.Modules.Helpers
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
    }
}
