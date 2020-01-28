using System.Collections.Generic;

namespace DiscordBot.LibraryData.ChatModels
{
    public class GuildModel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public List<GuildChannelModel> Channels { get; set; }/* = new List<GuildChannelModel>();*/
    }
}
