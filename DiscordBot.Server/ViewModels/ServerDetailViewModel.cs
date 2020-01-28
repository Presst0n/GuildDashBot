using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class ServerDetailViewModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public IEnumerable<TextChannelViewModel> TextChannels { get; set; }
    }
}
