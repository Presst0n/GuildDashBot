using Abstractions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Config;
using DiscordBot.Core.Data.DataAccess;
using DiscordBot.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class GuildBot : IGuildBot
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _botServices;
        private IServiceCollection _collection;
        private ILogger _logger;


        public GuildBot(ILogger logger)
        {
            _logger = logger;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Verbose
            });

            _cmdService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });

            _client.Log += ClientOnLog;
        }

        private Task ClientOnLog(LogMessage log)
        {
            _logger.Log($"[{log.Source}] = {log.Message}");
            return Task.CompletedTask;
        }

        public async Task Connect()
        {
            if (IsRunning()) { return; }
            if(_client != null && _cmdService != null)
            {
                await _client.LoginAsync(TokenType.Bot, GuildBotConfig.bot.discordToken);
                await _client.StartAsync();
                _client.Ready += OnDiscordClientReady;
            }
            else
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 50,
                    LogLevel = LogSeverity.Verbose
                });

                _cmdService = new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    CaseSensitiveCommands = false
                });

                await _client.LoginAsync(TokenType.Bot, GuildBotConfig.bot.discordToken);
                await _client.StartAsync();
            }

            //await _botServices.GetRequiredService<TwitchService>().InitializeConnection();
        }

        private async Task OnDiscordClientReady()
        {
            if (_botServices is null)
            {
                await InitializeServicesAsync(); //// I've managed to fix the problem where the bot multiplied the response to the command each time it was turned off and on.
            }
        }

        public async Task Stop()
        {
            if(!IsRunning()) { return; }

            await _client.StopAsync();
            await _client.LogoutAsync();
            _client.Ready -= OnDiscordClientReady;
            _collection.Clear();
            //await _botServices.GetRequiredService<CommandHandler>().UnsubscribeCommandHandlerEvents();
            //_cmdService.Modules.ToList().ForEach(x => _cmdService.RemoveModuleAsync(x));
        }

        public bool IsRunning() =>
            _client.ConnectionState == ConnectionState.Connected ||
            _client.ConnectionState == ConnectionState.Connecting;

        public async Task InitializeServicesAsync()
        {
            if (_collection is null)
            {
                _collection = new ServiceCollection(); 
            }

            _botServices = SetupBotServices();

            await _botServices.GetRequiredService<CommandHandler>().InitializeAsync();
            await _botServices.GetRequiredService<GuildRolesManager>().InitializeAsync();
            await _botServices.GetRequiredService<TwitchService>().InitializeConnection();
        }

        private IServiceProvider SetupBotServices()
            => _collection
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton<CommandHandler>()
            .AddSingleton<GuildRolesManager>()
            .AddSingleton<TwitchService>()
            .AddSingleton<RaiderioService>()
            .AddSingleton<IDataAccess, DataAccess>()
            .AddSingleton(_logger)
            .BuildServiceProvider();
    }
}
