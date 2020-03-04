using Abstractions;
using Discord;
using Discord.WebSocket;
using DiscordBot.Server.Bot.Config;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Bot
{
    public class GuildBot : IGuildBot
    {
        public event EventHandler OnBotReceivedMessage;

        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;

        public GuildBot(DiscordSocketClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Connect()
        {
            if (IsRunning()) { return; }
            SubscribeEvents();
            await _client.LoginAsync(TokenType.Bot, GuildBotConfig.bot.discordToken);
            await _client.StartAsync();
        }

        public async Task Stop()
        {
            if (!IsRunning()) { return; }
            await _client.LogoutAsync();
            await _client.StopAsync();
            UnsubsrcibeEvents();
        }

        private void SubscribeEvents()
        {
            _client.Log += ClientOnLog;
            _client.MessageReceived += ClientOnMessageReceived;
            _client.Disconnected += OnClientDisconnected;
        }

        private void UnsubsrcibeEvents()
        {
            _client.MessageReceived -= ClientOnMessageReceived;
            _client.Disconnected -= OnClientDisconnected;
            _client.Log -= ClientOnLog;
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

        private Task OnClientDisconnected(Exception arg)
        {
            _logger.Log($"Client: {arg.Message} Source: {arg.Source}");

            return Task.CompletedTask;
        }

        public bool IsRunning() =>
            _client.ConnectionState == ConnectionState.Connected ||
            _client.ConnectionState == ConnectionState.Connecting;

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
