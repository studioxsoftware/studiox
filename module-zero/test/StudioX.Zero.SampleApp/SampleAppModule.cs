using System.Reflection;
using StudioX.Modules;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.Authorization;
using StudioX.Zero.SampleApp.Configuration;
using StudioX.Zero.SampleApp.Features;

namespace StudioX.Zero.SampleApp
{
    [DependsOn(typeof(StudioXZeroCoreModule))]
    public class SampleAppModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Features.Providers.Add<AppFeatureProvider>();

            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            Configuration.MultiTenancy.IsEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
