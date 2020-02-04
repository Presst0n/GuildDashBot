using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DiscordBot.Server.Resources.Constants;

namespace DiscordBot.Server.EmailSender
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string msg)
        {
            var body = msg;
            var message = new MailMessage();

            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(EmailInfo.FROM_EMAIL_ACCOUNT);
            message.Subject = EmailInfo.EMAIL_SUBJECT_DEFAULT;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.UseDefaultCredentials = false;

                var credential = new NetworkCredential
                {
                    UserName = EmailInfo.FROM_EMAIL_ACCOUNT,
                    Password = EmailInfo.FROM_EMAIL_PASSWORD
                };

                smtp.Credentials = credential;
                smtp.Host = EmailInfo.SMTP_HOST_GMAIL;
                smtp.Port = Convert.ToInt32(EmailInfo.SMTP_PORT_GMAIL);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(message);
            }
        }

        public async Task SendEmailAsync(string email, string msg, string subject)
        {
            var body = msg;
            var message = new MailMessage();

            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(EmailInfo.FROM_EMAIL_ACCOUNT);
            message.Subject = !string.IsNullOrEmpty(subject) ? subject : EmailInfo.EMAIL_SUBJECT_DEFAULT;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.UseDefaultCredentials = false;

                var credential = new NetworkCredential
                {
                    UserName = EmailInfo.FROM_EMAIL_ACCOUNT,
                    Password = EmailInfo.FROM_EMAIL_PASSWORD
                };

                smtp.Credentials = credential;
                smtp.Host = EmailInfo.SMTP_HOST_GMAIL;
                smtp.Port = Convert.ToInt32(EmailInfo.SMTP_PORT_GMAIL);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(message);
            }
        }
    }
}
