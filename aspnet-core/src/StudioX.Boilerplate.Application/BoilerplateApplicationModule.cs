using StudioX.AutoMapper;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Configuration;

namespace StudioX.Boilerplate
{
    [DependsOn(
        typeof(BoilerplateCoreModule), 
        typeof(StudioXAutoMapperModule))]
    public class BoilerplateApplicationModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BoilerplateAuthorizationProvider>();

            Configuration.Settings.Providers.Add < BoilerplateSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateApplicationModule).GetAssembly());
        }
    }
}