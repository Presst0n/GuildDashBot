using DiscordBot.Core.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;

namespace DiscordBot.Core.TwitchApi
{
    public class TwitchApiProvider
    {
        public Task<TwitchAPI> GetApi()
        {
            var api = new TwitchAPI();
            api.Settings.ClientId = GuildBotConfig.bot.twitchClient;
            api.Settings.AccessToken = GuildBotConfig.bot.twitchToken;

            return Task.FromResult(api);
        }
    }
}
