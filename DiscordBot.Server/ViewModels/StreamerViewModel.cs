using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class StreamerViewModel
    {
        [Required]
        public string StreamerLogin { get; set; }
        public string StreamerId { get; set; }
        public string UniqueID { get; set; }
        public bool IsStreaming { get; set; }
        public string PlayedGame { get; set; }
        public int Viewers { get; set; }
        public string ProfileImage { get; set; }
        public string StreamTitle { get; set; }
        public long TotalFollows { get; set; }
        [Required]
        public string Url { get; set; }
        public string LiveIndicator { get; set; }
    }
}
