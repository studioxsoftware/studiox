#if NET46
using StudioX.Net.Mail.Smtp;
using NSubstitute;
using Xunit;

namespace StudioX.Tests.Net.Mail
{
    public class SmtpEmailSenderTests
    {
        private readonly SmtpEmailSender smtpEmailSender;

        public SmtpEmailSenderTests()
        {
            var configuration = Substitute.For<ISmtpEmailSenderConfiguration>();

            configuration.DefaultFromAddress.Returns("...");
            configuration.DefaultFromDisplayName.Returns("...");

            configuration.Host.Returns("...");
            configuration.Port.Returns(25);

            //configuration.Domain.Returns("...");
            configuration.UserName.Returns("...");
            configuration.Password.Returns("...");

            //configuration.EnableSsl.Returns(false);
            //configuration.UseDefaultCredentials.Returns(false);

            smtpEmailSender = new SmtpEmailSender(configuration);
        }

        //[Fact] //Need to set configuration before executing this test
        public void TestSendEmail()
        {
            smtpEmailSender.Send(
                "...", 
                "Test email", 
                "An email body"
                );
        }
    }
}
#endif