using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DiscordBot.Server.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;
using DiscordBot.LibraryData.Models;

namespace DiscordBot.Server.Bot.Modules
{
    public class Twitch : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _configuration;

        public Twitch(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Command("set_twitch")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetChannelForTwitchNotification()
        {
            var channel = Context.Channel.Id;
            await SetId(channel);
            await ReplyAsync("Powiadomienia o streamach będą wysyłane na tym kanale.");
        }

        private async Task SetId(ulong id)
        {
            using (var context = new BotDbContext(_configuration))
            {
                if (context.TwitchChannel.Where(x => x.Channel_Id != id).Count() >= 1)
                {
                    ChannelDbModel ChannelFromDb = context.TwitchChannel.FirstOrDefault();
                    context.TwitchChannel.Remove(ChannelFromDb);
                    context.TwitchChannel.Add(new ChannelDbModel
                    {
                        Channel_Id = id,
                    });

                }
                else if (context.TwitchChannel.Where(x => x.Channel_Id == id).Count() < 1)
                {
                    context.TwitchChannel.Add(new ChannelDbModel
                    {
                        Channel_Id = id
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
