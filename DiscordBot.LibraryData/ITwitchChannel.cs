using DiscordBot.LibraryData.Models;
using System.Threading.Tasks;

namespace DiscordBot.LibraryData
{
    public interface ITwitchChannel
    {
        Task<ChannelDbModel> GetId();
        Task SetId(ulong id);
    }
}
