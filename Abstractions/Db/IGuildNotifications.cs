using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Db
{
    public interface IGuildNotifications
    {
        Task<GuildNotificationDbModel> GetNotificationStatus();
        Task SetNotificationStatus(GuildNotificationDbModel input);
    }
}
