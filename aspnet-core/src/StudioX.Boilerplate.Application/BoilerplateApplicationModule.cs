using System.Reflection;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

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

            Configuration.Settings.Providers.Add<BoilerplateSettingProvider>();
        }

        public override void Initialize()
        {
            Assembly thisAssembly = typeof(BoilerplateApplicationModule).GetAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.StudioXAutoMapper().Configurators.Add(cfg =>
            {
                //Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg.AddProfiles(thisAssembly);
            });
        }
    }
}