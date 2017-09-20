using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Net.Mail;
using Microsoft.AspNet.Identity;

namespace StudioX.IdentityFramework
{
    public class IdentityEmailMessageService : IIdentityMessageService, ITransientDependency
    {
        private readonly IEmailSender _emailSender;

        public IdentityEmailMessageService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public virtual Task SendAsync(IdentityMessage message)
        {
            return _emailSender.SendAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
