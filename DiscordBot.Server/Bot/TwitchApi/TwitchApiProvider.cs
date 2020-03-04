using DiscordBot.Server.Bot.Config;
using System.Threading.Tasks;
using TwitchLib.Api;

namespace DiscordBot.Server.Bot.TwitchApi
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
