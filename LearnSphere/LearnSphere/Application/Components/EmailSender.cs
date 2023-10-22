using Microsoft.Extensions.Options;
using LearnSphere.Models.ComponentModels;
using LearnSphere.Models.ConfigurationModels;
using System.Net.Mail;
using System.Net;
using LearnSphere.Application.Contracts;

namespace LearnSphere.Application.Components
{
    public class EmailSender:IEmailSender
    {
        public EmailSender(IOptions<SmtpConfiguration> smtpConfiguration)
        {
            SmtpConfiguration = smtpConfiguration.Value;
        }
        readonly SmtpConfiguration SmtpConfiguration;
        public void Send(Email email)
        {
            var client = new SmtpClient
            {
                Host = SmtpConfiguration.Server,
                Port = SmtpConfiguration.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials =
                new NetworkCredential(SmtpConfiguration.UserName, SmtpConfiguration.Password)
            };
            var message =
                new MailMessage
                {
                    From = new MailAddress(SmtpConfiguration.Sender),
                    Subject = email.Subject,
                    Body = email.Body
                };

            message.To.Add(new MailAddress(email.Recipient));

            client.Send(message);

        }
    }
}

