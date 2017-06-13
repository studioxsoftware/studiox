using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Net.Mail.Smtp;
using MailKit.Net.Smtp;

namespace StudioX.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration)
        {
            this.smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
        }

        public virtual SmtpClient Build()
        {
            var client = new SmtpClient();

            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(
                smtpEmailSenderConfiguration.Host,
                smtpEmailSenderConfiguration.Port,
                smtpEmailSenderConfiguration.EnableSsl
            );

            var userName = smtpEmailSenderConfiguration.UserName;
            if (!userName.IsNullOrEmpty())
            {
                client.Authenticate(
                    smtpEmailSenderConfiguration.UserName, 
                    smtpEmailSenderConfiguration.Password
                );
            }
        }
    }
}