using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.LibraryData.Models
{
    public class StreamerDbModel
    {
        [Key]
        public string UniqueID { get; set; }
        public string StreamerLogin { get; set; }
        public string StreamerId { get; set; }
        public bool SentMessage { get; set; }
        public bool IsStreaming { get; set; }
        public string UrlAddress { get; set; }
        public string PlayedGame { get; set; }
        public int Viewers { get; set; }
        public string ProfileImage { get; set; }
        public string StreamTitle { get; set; }
        public long TotalFollows { get; set; }
    }
}
