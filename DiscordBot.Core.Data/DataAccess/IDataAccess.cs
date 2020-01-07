using DiscordBot.LibraryData;
using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Data.DataAccess
{
    public interface IDataAccess : ITwitchStreamers, ITwitchChannel, IGuildNotifications
    {
        Task AddAsync(GuildMessageDbModel message);
        Task<GuildMessageDbModel> GetMessageById(string id);
        Task<IEnumerable<GuildMessageDbModel>> GetMessages();
    }
}
