using System.Threading.Tasks;

namespace DiscordBot.Server.Bot.Services
{
    public interface ITwitchLiveMonitor
    {
        Task StartLiveMonitor();
        Task CheckNewChannelsToMonitor();
        bool IsEnabled();
    }
}