using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Extensions;

namespace StudioX.Net.Mail.Smtp
{
    /// <summary>
    /// Used to send emails over SMTP.
    /// </summary>
    public class SmtpEmailSender : EmailSenderBase, ISmtpEmailSender, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration configuration;

        /// <summary>
        /// Creates a new <see cref="SmtpEmailSender"/>.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SmtpEmailSender(ISmtpEmailSenderConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
        }

        public SmtpClient BuildClient()
        {
            var host = configuration.Host;
            var port = configuration.Port;

            var smtpClient = new SmtpClient(host, port);
            try
            {
                if (configuration.EnableSsl)
                {
                    smtpClient.EnableSsl = true;
                }

                if (configuration.UseDefaultCredentials)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;

                    var userName = configuration.UserName;
                    if (!userName.IsNullOrEmpty())
                    {
                        var password = configuration.Password;
                        var domain = configuration.Domain;
                        smtpClient.Credentials = !domain.IsNullOrEmpty()
                            ? new NetworkCredential(userName, password, domain)
                            : new NetworkCredential(userName, password);
                    }
                }

                return smtpClient;
            }
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using (var smtpClient = BuildClient())
            {
                await smtpClient.SendMailAsync(mail);
            }
        }

        protected override void SendEmail(MailMessage mail)
        {
            using (var smtpClient = BuildClient())
            {
                smtpClient.Send(mail);
            }
        }
    }
}
