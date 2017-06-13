using System.Threading.Tasks;
using StudioX.Net.Mail.Smtp;
using NSubstitute;

#if NET46
using System.Net.Mail;
#endif

namespace StudioX.MailKit.Tests
{
    public class MailKitEmailSenderTests
    {
        //[Fact]
        public void ShouldSend()
        {
            var mailSender = CreateMailKitEmailSender();

            mailSender.Send("from", "to", "subject", "body", true);
        }

        //[Fact]
        public async Task ShouldSendAsync()
        {
            var mailSender = CreateMailKitEmailSender();

            await mailSender.SendAsync("from", "to", "subject", "body", true);
        }

#if NET46
        //[Fact]
        public async Task ShouldSendMailMessage()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from", "to", "subject", "body")
            { IsBodyHtml = true };

            await mailSender.SendAsync(mailMessage);
        }

        //[Fact]
        public void ShouldSendMailMessageAsync()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from", "to", "subject", "body")
            { IsBodyHtml = true };

            mailSender.Send(mailMessage);
        }
#endif
        private static MailKitEmailSender CreateMailKitEmailSender()
        {
            var mailConfig = Substitute.For<ISmtpEmailSenderConfiguration>();

            mailConfig.Host.Returns("stmpservername");
            mailConfig.UserName.Returns("mailserverusername");
            mailConfig.Password.Returns("mailserverpassword");
            mailConfig.Port.Returns(587);
            mailConfig.EnableSsl.Returns(false);

            var mailSender = new MailKitEmailSender(mailConfig, new DefaultMailKitSmtpBuilder(mailConfig));
            return mailSender;
        }
    }
}
