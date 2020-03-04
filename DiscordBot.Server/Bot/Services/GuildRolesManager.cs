using Abstractions;
using Abstractions.Db;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Bot.Services
{
    public class GuildRolesManager : IGuildRolesManager
    {
        private readonly DiscordSocketClient _client;
        private readonly IGuildBot _bot;
        private readonly IBotDbService _botDbService;

        public GuildRolesManager(DiscordSocketClient client, IGuildBot bot, IBotDbService botDbService)
        {
            _client = client;
            _bot = bot;
            _botDbService = botDbService;
        }

        public Task InitializeAsync()
        {
            _client.UserJoined += AssignRoleToNewMember;
            _client.GuildMemberUpdated += OnGuildMemberUpdate;
            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _client.UserJoined -= AssignRoleToNewMember;
            _client.GuildMemberUpdated -= OnGuildMemberUpdate;

            return Task.CompletedTask;
        }

        private async Task OnGuildMemberUpdate(SocketGuildUser userBefore, SocketGuildUser userAfter)
        {
            if (!_bot.IsRunning()) { return; }
            var notification = await _botDbService.GetNotificationStatus();

            if (notification is null) { return; }

            if (notification.Notify == true)
            {
                if (userBefore.Roles.Where(x => x.Name == "Raider").Count() == 1)
                {
                    return;
                }

                if (userAfter.Roles.Where(x => x.Name == "Raider").Count() == 1)
                {
                    var user = userAfter as IGuildUser;
                    var msg = await _botDbService.GetMessageById("raider_msg");

                    await user.SendMessageAsync(msg.Message);
                }
            }
        }

        private async Task AssignRoleToNewMember(SocketGuildUser guildUser)
        {
            if (!_bot.IsRunning()) { return; }

            var user = guildUser as IGuildUser;
            var role = guildUser.Guild.Roles.FirstOrDefault(x => x.Name == "Gość");
            if (role is null) { return; }

            await user.AddRoleAsync(role);
            var notificationStatus = await _botDbService.GetNotificationStatus();

            if (notificationStatus.Notify == true)
            {
                await NotifyNewMember(user);
            }
        }

        private async Task NotifyNewMember(IGuildUser user)
        {
            var msg = await _botDbService.GetMessageById("welcome_msg");
            if (msg is null) { return; }
            await user.SendMessageAsync(msg.Message);
        }
    }
}
