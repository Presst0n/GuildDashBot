using DiscordBot.LibraryData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Db
{
    public interface IBotDbService
    {
        Task<ChannelDbModel> GetId();
        Task<GuildMessageDbModel> GetMessageById(string id);
        Task<GuildNotificationDbModel> GetNotificationStatus();
        Task<IEnumerable<StreamerDbModel>> GetStreamers();
    }
}