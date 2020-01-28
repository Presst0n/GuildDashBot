using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class ChatViewModel
    {
        public ServerDetailViewModel ActiveServer { get; set; }
        public TextChannelViewModel ActiveChannel { get; set; }
        public IEnumerable<ChatMessageViewModel> MessageBuffer { get; set; }
        public IEnumerable<ServerDetailViewModel> AvailableServers { get; set; }
    }
}
