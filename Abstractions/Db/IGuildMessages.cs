using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Db
{
    public interface IGuildMessages
    {
        Task<IEnumerable<GuildMessageDbModel>> GetMessages();
        Task<GuildMessageDbModel> GetMessageById(string id);
        Task AddAsync(GuildMessageDbModel message);
    }
}
