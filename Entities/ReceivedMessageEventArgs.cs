using System;

namespace Entities
{
    public class ReceivedMessageEventArgs : EventArgs
    {
        public ulong SenderId { get; set; }
        public string SenderAvatarUrl { get; set; }
        public string SenderUsername { get; set; }
        public int SenderReputation { get; set; }
        public string Message { get; set; }
        public string ChannelName { get; set; }
        public ulong ChannelId { get; set; }
    }
}
