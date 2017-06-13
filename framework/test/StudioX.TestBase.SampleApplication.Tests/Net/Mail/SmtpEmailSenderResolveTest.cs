using StudioX.Net.Mail;
using StudioX.Net.Mail.Smtp;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Net.Mail
{
    public class SmtpEmailSenderResolveTest : StudioXIntegratedTestBase<StudioXKernelModule>
    {
        [Fact]
        public void ShouldResolveEmailSenders()
        {
            Resolve<IEmailSender>().ShouldBeOfType(typeof(SmtpEmailSender));
            Resolve<ISmtpEmailSender>().ShouldBeOfType(typeof(SmtpEmailSender));
        }
    }
}
