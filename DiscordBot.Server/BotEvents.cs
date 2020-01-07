using Abstractions;
using DiscordBot.Core;
using DiscordBot.Server.Hubs;
using Entities;
using Microsoft.AspNetCore.SignalR;
using System;

namespace DiscordBot.Server
{
    public class BotEvents
    {
        private readonly IHubContext<BotHub> _botHubContext;

        public BotEvents(ILogger logger, IGuildBot bot, IHubContext<BotHub> botHubContext)
        {
            _botHubContext = botHubContext;

            logger.OnLog += OnNewLog;
        }

        private async void OnNewLog(object sender, EventArgs e)
        {
            if(!(e is LogEventArgs args)) { return; }
            await _botHubContext.Clients.All.SendCoreAsync("NewLogAdded", new object[] { args.NewMessage });
        }
    }
}
