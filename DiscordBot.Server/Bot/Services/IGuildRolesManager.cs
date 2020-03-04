using System.Threading.Tasks;

namespace DiscordBot.Server.Bot.Services
{
    public interface IGuildRolesManager
    {
        Task InitializeAsync();
        Task Clear();
    }
}