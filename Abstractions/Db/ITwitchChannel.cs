using DiscordBot.LibraryData.Models;
using System.Threading.Tasks;

namespace Abstractions.Db
{
    public interface ITwitchChannel
    {
        Task<ChannelDbModel> GetId();
        Task SetId(ulong id);
    }
}
