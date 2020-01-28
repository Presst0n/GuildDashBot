using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IGuildBot
    {
        event EventHandler OnBotReceivedMessage;

        Task Connect();
        IEnumerable<ServerDetail> GetAvailableServers();
        Task<IEnumerable<ChatMessage>> GetMessageBufferFor(ulong serverId, ulong channelId);
        ServerDetail GetServerDetailFromId(ulong serverId);
        TextChannel GetTextChannelDetailFromId(ulong serverId, ulong channelId);
        bool IsRunning();
        Task SayInChannel(ulong serverId, ulong channelId, string message);
        Task Stop();
    }
}