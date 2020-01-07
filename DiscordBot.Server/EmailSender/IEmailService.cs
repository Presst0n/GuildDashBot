using System.Threading.Tasks;

namespace DiscordBot.Server.EmailSender
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string msg);
        Task<bool> SendEmailAsync(string email, string msg, string subject);
    }
}