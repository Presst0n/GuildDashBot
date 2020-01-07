using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class GuildNotificationViewModel
    {
        public string NotifyId { get; set; }
        public bool Notify { get; set; }
    }
}
