using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Db
{
    public interface ITwitchStreamers
    {
        Task AddStreamerAsync(StreamerDbModel streamer);
        Task DeleteStreamerAsync(StreamerDbModel streamer);
        Task<IEnumerable<StreamerDbModel>> GetStreamers();
        Task<StreamerDbModel> GetStreamerByUniqueId(string id);
        Task<StreamerDbModel> GetStreamerById(string id);
    }
}
