using Abstractions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Hubs
{
    public class BotHub : Hub
    {
        private readonly IGuildBot _bot;

        public BotHub(IGuildBot bot)
        {
            _bot = bot;
        }

        public Task ToggleBotConnection()
        {
            if (_bot.IsRunning())
            {
                _bot.Stop();
            }
            else
            {
                _bot.Connect();
            }

            return Task.CompletedTask;
        }
    }
}
