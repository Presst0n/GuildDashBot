using Abstractions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IGuildBot _bot;

        public ChatHub(IGuildBot bot)
        {
            _bot = bot;
        }

        public async Task SayInChannel(string location, string message)
        {
            var locationSplit = location.Split('/');
            if (locationSplit.Length != 2) { return; }
            var serverParsed = ulong.TryParse(locationSplit[0], out var serverId);
            var channelParsed = ulong.TryParse(locationSplit[1], out var channelId);
            if (!serverParsed || !channelParsed) { return; }
            await _bot.SayInChannel(serverId, channelId, message);
        }
    }
}
