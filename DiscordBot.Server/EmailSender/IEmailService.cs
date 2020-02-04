using System.Threading.Tasks;

namespace DiscordBot.Server.EmailSender
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string msg);
        Task SendEmailAsync(string email, string msg, string subject);
    }
}