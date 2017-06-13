using StudioX.Dependency;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.Net.Mail;
using StudioX.Reflection.Extensions;

namespace StudioX.MailKit
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXMailKitModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IEmailSender, MailKitEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXMailKitModule).GetAssembly());
        }
    }
}
