using System.Collections.Generic;

namespace Entities
{
    public class ChatMessage
    {
        public ulong SenderId { get; set; }
        public string SenderAvatarUrl { get; set; }
        public string SenderUsername { get; set; }
        public int SenderReputation { get; set; }
        public string Message { get; set; }
        public string ChannelName { get; set; }
        public ulong ChannelId { get; set; }
        public IEnumerable<string> FileLinks { get; set; }
    }
}
