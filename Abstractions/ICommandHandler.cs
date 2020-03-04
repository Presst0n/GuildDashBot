using System.Threading.Tasks;

namespace Abstractions
{
    public interface ICommandHandler
    {
        Task InitializeAsync();
        Task Clear();
    }
}