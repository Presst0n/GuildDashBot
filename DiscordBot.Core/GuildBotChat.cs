using Discord;
using Discord.WebSocket;
using DiscordBot.LibraryData.ChatModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class GuildBotChat
    {
        private readonly DiscordSocketClient _client;

        public GuildBotChat(DiscordSocketClient client)
        {
            _client = client;
        }

        // TODO - Continue from here. Create a some guild model with properties to map from socket guild collection. 
        // I want to display list of guilds in web dashboard, and after that show list of available channels in this concrete guild. 
        // Thanks to that I'll be able to choose whatever text channel I want and 
        // read or send message via front end. 

        public Task<IEnumerable<GuildModel>> GetGuilds()
        {
            var guildModels = new List<GuildModel>();

            var guilds = _client.Guilds.ToList();

            for (int i = 0; i < guilds.Count; i++)
            {
                List<GuildChannelModel> Channels = new List<GuildChannelModel>();

                foreach (var channel in guilds[i].Channels)
                {
                    Channels.Add(new GuildChannelModel()
                    {
                        Id = channel.Id,
                        Name = channel.Name
                    });
                }

                guildModels.Add(new GuildModel
                {
                    Id = guilds[i].Id,
                    Name = guilds[i].Name,
                    Channels = Channels
                });

            }

            return Task.FromResult(guildModels.AsEnumerable());
        }

        public async Task SendMessage(ulong channelId, string msg)
        {
            var c = _client.GetChannel(channelId) as IMessageChannel;
            await c.SendMessageAsync(msg);
        }
    }
}
