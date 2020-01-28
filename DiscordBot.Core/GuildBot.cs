using Abstractions;
using Abstractions.Db;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Config;
using DiscordBot.Core.Services;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class GuildBot : IGuildBot
    {
        public event EventHandler OnBotReceivedMessage;

        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _botServices;
        private IServiceCollection _collection;
        private readonly ILogger _logger;
        private readonly IDataAccess _dataAccess;

        public GuildBot(ILogger logger, IDataAccess dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;

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
            _client.MessageReceived += ClientOnMessageReceived;
        }

        private Task ClientOnMessageReceived(SocketMessage message)
        {
            var args = new ReceivedMessageEventArgs
            {
                Message = message.Content,
                SenderUsername = message.Author.Username,
                ChannelName = message.Channel.Name,
                SenderAvatarUrl = message.Author.GetAvatarUrl(),
                SenderId = message.Author.Id,
                ChannelId = message.Channel.Id,
                SenderReputation = 0 // Not implemented
            };

            OnBotReceivedMessage?.Invoke(this, args);
            return Task.CompletedTask;
        }

        private Task ClientOnLog(LogMessage log)
        {
            _logger.Log($"[{log.Source}] = {log.Message}");
            return Task.CompletedTask;
        }

        public async Task Connect()
        {
            if (IsRunning()) { return; }

            if (_client == null && _cmdService == null)
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
            }

            await _client.LoginAsync(TokenType.Bot, GuildBotConfig.bot.discordToken);
            await _client.StartAsync();
            _client.Ready += OnDiscordClientReady;
        }

        private async Task OnDiscordClientReady()
        {
            if (_botServices is null)
            {
                await InitializeServicesAsync();
            }
        }

        public async Task Stop()
        {
            if (!IsRunning()) { return; }

            await _client.StopAsync();
            await _client.LogoutAsync();
            _client.Ready -= OnDiscordClientReady;
            _collection.Clear();
        }

        public bool IsRunning() =>
            _client.ConnectionState == ConnectionState.Connected ||
            _client.ConnectionState == ConnectionState.Connecting;

        private async Task InitializeServicesAsync()
        {
            if (_collection is null)
            {
                _collection = new ServiceCollection();
            }

            _botServices = SetupBotServices();

            await _botServices.GetRequiredService<CommandHandler>().InitializeAsync();
            await _botServices.GetRequiredService<GuildRolesManager>().InitializeAsync();
            await _botServices.GetRequiredService<TwitchService>().InitializeService();
        }

        private IServiceProvider SetupBotServices()
            => _collection
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton<CommandHandler>()
            .AddSingleton<GuildRolesManager>()
            .AddSingleton<TwitchService>()
            .AddSingleton<RaiderioService>()
            .AddSingleton(_dataAccess)
            .AddSingleton(_logger)
            .BuildServiceProvider();

        public IEnumerable<ServerDetail> GetAvailableServers()
            => _client.Guilds.Select(guild => new ServerDetail
            {
                Name = guild.Name,
                AvatarUrl = guild.IconUrl,
                Id = guild.Id,
                TextChannels = guild.TextChannels.Select(c => c).Where(t => t.Users.Any(u => u.Username == _client.CurrentUser.Username)).Select(ToTextChannel)
            });

        public ServerDetail GetServerDetailFromId(ulong serverId)
        {
            var server = _client.GetGuild(serverId);
            if (server is null) { return null; }
            return new ServerDetail
            {
                AvatarUrl = server.IconUrl,
                Name = server.Name,
                Id = server.Id,
                TextChannels = server.TextChannels.Select(ToTextChannel)
            };
        }

        private static TextChannel ToTextChannel(SocketTextChannel textChannel)
            => new TextChannel
            {
                Name = textChannel.Name,
                Id = textChannel.Id
            };

        public TextChannel GetTextChannelDetailFromId(ulong serverId, ulong channelId)
        {
            var channel = _client.GetGuild(serverId).GetTextChannel(channelId);
            return new TextChannel
            {
                Name = channel.Name,
                Id = channel.Id
            };
        }

        public async Task<IEnumerable<ChatMessage>> GetMessageBufferFor(ulong serverId, ulong channelId)
        {
            var guild = _client.GetGuild(serverId);
            var channel = guild.GetTextChannel(channelId);

            var messages = await channel.GetMessagesAsync(10).FlattenAsync();

            return messages.Select(m => new ChatMessage
            {
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                SenderId = m.Author.Id,
                SenderUsername = m.Author.Username,
                SenderAvatarUrl = m.Author.GetAvatarUrl(),
                SenderReputation = 0,
                Message = m.Content,
                FileLinks = m.Attachments.Select(a => a.Url)
            }).Reverse();
        }

        public async Task SayInChannel(ulong serverId, ulong channelId, string message)
        {
            var guild = _client.GetGuild(serverId);
            var channel = guild.GetTextChannel(channelId);
            await channel.SendMessageAsync(message);
        }
    }
}
