using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class OverviewViewModel
    {
        public bool BotIsRunning { get; set; }
        public IEnumerable<string> LogBuffer { get; set; }
    }
}
