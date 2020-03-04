using Abstractions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Server.Bot.Config;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Server.Bot
{
    public class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services, ILogger logger)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync;
        }

        public async Task Clear()
        {
            var t = _cmdService.Modules.ToList();

            for (int i = 0; i < t.Count; i++)
            {
                await _cmdService.RemoveModuleAsync(t[i]);
            }

            _client.MessageReceived -= HandleMessageAsync;
            _cmdService.Log -= LogAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;

            if (!(socketMessage is SocketMessage message))
                return;

            var userMessage = socketMessage as SocketUserMessage;

            if (userMessage is null)
                return;

            if (!(userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos) || userMessage.HasStringPrefix(GuildBotConfig.bot.cmdPrefix, ref argPos)))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _cmdService.ExecuteAsync(context, argPos, _services);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            _logger.Log($"{logMessage.Source} : {logMessage.Message}");

            //if (logMessage.Exception != null)
            //{
            //    _logger.Log($"{logMessage.Exception.Message} : {logMessage.Exception.StackTrace}");
            //}

            return Task.CompletedTask;
        }
    }
}
