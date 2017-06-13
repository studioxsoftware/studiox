using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Net.Mail;
using Microsoft.AspNet.Identity;

namespace StudioX.IdentityFramework
{
    public class IdentityEmailMessageService : IIdentityMessageService, ITransientDependency
    {
        private readonly IEmailSender emailSender;

        public IdentityEmailMessageService(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public virtual Task SendAsync(IdentityMessage message)
        {
            return emailSender.SendAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
