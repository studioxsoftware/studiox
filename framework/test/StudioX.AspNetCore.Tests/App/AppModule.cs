using StudioX.AspNetCore.App.MultiTenancy;
using StudioX.AspNetCore.TestBase;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mocks;
using StudioX.Auditing;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Reflection.Extensions;

namespace StudioX.AspNetCore.App
{
    [DependsOn(typeof(StudioXAspNetCoreTestBaseModule))]
    public class AppModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            Configuration.ReplaceService<IAuditingStore, MockAuditingStore>();
            Configuration.ReplaceService<ITenantStore, TestTenantStore>();

            Configuration
                .Modules.StudioXAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AppModule).GetAssembly()
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AppModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var localizationConfiguration = IocManager.IocContainer.Resolve<ILocalizationConfiguration>();
            localizationConfiguration.Languages.Add(new LanguageInfo("en-US", "English", isDefault: true));
            localizationConfiguration.Languages.Add(new LanguageInfo("it", "Italian"));
        }
    }
}
