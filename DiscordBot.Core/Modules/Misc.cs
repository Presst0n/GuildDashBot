using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeMessages(int msgCount)
        {
            var cachedMessages = Context.Channel.GetCachedMessages(50);
            int numberValue = -1;

            foreach (var msg in cachedMessages)
            {
                if (msgCount > numberValue)
                {
                    await msg.DeleteAsync();
                    numberValue++;
                }
                else
                {
                    return;
                }
            }
        }

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeMessages(IGuildUser user)
        {
            var cachedMessages = Context.Channel.GetCachedMessages(50);

            foreach (var msg in cachedMessages)
            {
                if (msg.Author.Id == user.Id)
                {
                    await msg.DeleteAsync();
                }
            }

            await Context.Channel.DeleteMessageAsync(Context.Message.Id);
        }

        [Command("pinned")]
        public async Task Pinned()
        {
            //var user = Context.User as SocketGuildUser;
            await Context.Channel.DeleteMessageAsync(Context.Message.Id);
            var pinnedMsg = await Context.Channel.GetPinnedMessagesAsync();
            await ReplyAsync($"Oto wszystkie przypięte wiadomości na tym kanale:{Environment.NewLine}{Environment.NewLine}");

            foreach (var item in pinnedMsg)
            {
                await ReplyAsync($"{Environment.NewLine}{item.ToString()}");
            }

        }


    }
}
