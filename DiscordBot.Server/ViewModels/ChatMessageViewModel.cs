using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.ViewModels
{
    public class ChatMessageViewModel
    {
        public ulong SenderId { get; set; }
        public string SenderAvatarUrl { get; set; }
        public string SenderUsername { get; set; }
        public int SenderReputation { get; set; }
        public string Message { get; set; }
        public string ChannelName { get; set; }
        public IEnumerable<string> FileLinks { get; set; }
    }
}
