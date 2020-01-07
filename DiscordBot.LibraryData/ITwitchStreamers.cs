using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.LibraryData
{
    public interface ITwitchStreamers
    {
        Task AddStreamerAsync(StreamerDbModel streamer);
        Task DeleteStreamerAsync(StreamerDbModel streamer);
        Task<IEnumerable<StreamerDbModel>> GetStreamers();
        Task<StreamerDbModel> GetStreamerById(string id);
    }
}
