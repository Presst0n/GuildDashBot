using System.Threading.Tasks;

namespace Abstractions
{
    public interface IGuildBot
    {
        Task Connect();
        bool IsRunning();
        Task Stop();
    }
}