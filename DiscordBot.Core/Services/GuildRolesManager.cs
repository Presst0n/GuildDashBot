using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Data.DataAccess;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Core.Services
{
    public class GuildRolesManager
    {
        private readonly DiscordSocketClient _client;
        private readonly IDataAccess _database;

        public GuildRolesManager(DiscordSocketClient client, IDataAccess database)
        {
            _client = client;
            _database = database;
        }

        public Task InitializeAsync()
        {
            _client.UserJoined += AssignRoleToNewMember;
            _client.GuildMemberUpdated += _client_GuildMemberUpdated;
            return Task.CompletedTask;
        }

        private async Task _client_GuildMemberUpdated(SocketGuildUser userBefore, SocketGuildUser userAfter)
        {
            var notification = await _database.GetNotificationStatus();

            if (notification.Notify == true)
            {
                if (userBefore.Roles.Where(x => x.Name == "Raider").Count() == 1)
                {
                    return;
                }

                if (userAfter.Roles.Where(x => x.Name == "Raider").Count() == 1)
                {
                    var user = userAfter as IGuildUser;
                    var msg = await _database.GetMessageById("raider_msg");

                    await UserExtensions.SendMessageAsync(user, msg.Message);
                }
            }
        }

        private async Task AssignRoleToNewMember(SocketGuildUser guildUser)
        {
            var user = guildUser as IGuildUser;
            var role = guildUser.Guild.Roles.FirstOrDefault(x => x.Name == "Gość");
            await user.AddRoleAsync(role);
            var notificationStatus = _database.GetNotificationStatus();

            if (notificationStatus.Result.Notify == true)
            {
                await NotifyNewMember(user);
            }
        }

        public async Task NotifyNewMember(IGuildUser user)
        {
            var msg = await _database.GetMessageById("welcome_msg");
            await UserExtensions.SendMessageAsync(user, msg.Message);
        }

    }
}
