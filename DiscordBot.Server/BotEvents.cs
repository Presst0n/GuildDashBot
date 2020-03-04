using Abstractions;
using DiscordBot.Server;
using DiscordBot.Server.Hubs;
using Entities;
using Microsoft.AspNetCore.SignalR;
using System;

namespace DiscordBot.Server
{
    public class BotEvents
    {
        private readonly IHubContext<BotHub> _botHubContext;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public BotEvents(ILogger logger, IGuildBot bot, IHubContext<BotHub> botHubContext, IHubContext<ChatHub> chatHubContext)
        {
            _botHubContext = botHubContext;
            _chatHubContext = chatHubContext;
            logger.OnLog += OnNewLog;
            bot.OnBotReceivedMessage += OnBotReceivedMessage;
        }

        private async void OnBotReceivedMessage(object sender, EventArgs e)
        {
            if (!(e is ReceivedMessageEventArgs args)) { return; }
            await _chatHubContext.Clients.All.SendCoreAsync($"Chat-{args.ChannelId}", new object[] { args });
            await _chatHubContext.Clients.All.SendCoreAsync("ChatGlobal", new object[] { args });
        }

        private async void OnNewLog(object sender, EventArgs e)
        {
            if(!(e is LogEventArgs args)) { return; }
            await _botHubContext.Clients.All.SendCoreAsync("NewLogAdded", new object[] { args.NewMessage });
        }
    }
}
