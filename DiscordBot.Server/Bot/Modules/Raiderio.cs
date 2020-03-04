using Discord.Commands;
using DiscordBot.Server.Bot.Extensions;
using DiscordBot.Server.Bot.Modules.Helpers;
using DiscordBot.Server.Bot.Services;
using System.Threading.Tasks;

namespace DiscordBot.Server.Bot.Modules
{
    public class Raiderio : ModuleBase<SocketCommandContext>
    {
        private RaiderioService _raiderioService;

        public Raiderio(RaiderioService raiderioService)
        {
            _raiderioService = raiderioService;
        }

        [Command("progress")]
        public async Task ShowGuildProgress()
        {
            var output = await _raiderioService.GetGuildRaidProgAsync();
            await ReplyAsync(null, false, output.GetEmbededGuildProgressMsg());
        }

        [Command("recent")]
        public async Task ShowRecentRuns(string name, string realm, string region)
        {
            var output = await _raiderioService.GetRecentRunsAsync(RaiderioHelper.ConvertToArray(name, realm, region));
            await ReplyAsync(null, false, output.GetEmbededRecentMpRunsMsg());
        }

        [Command("best")]
        public async Task ShowBestRuns(string name, string realm, string region, int count)
        {
            var output = await _raiderioService.GetBestRunsAsync(RaiderioHelper.ConvertToArray(name, realm, region), count);
            await ReplyAsync(null, false, output.GetEmbededBestMpRunsMsg());
        }

        [Command("charinfo")]
        public async Task ShowCharacter(string name, string realm, string region)
        {
            var output = await _raiderioService.GetCharacterAsync(RaiderioHelper.ConvertToArray(name, realm, region));
            await ReplyAsync(null, false, output.GetEmbededCharacterMsg());
        }
    }
}
