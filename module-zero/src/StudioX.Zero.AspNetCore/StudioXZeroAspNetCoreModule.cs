using System.Reflection;
using StudioX.Authorization.Users;
using StudioX.Configuration.Startup;
using StudioX.Modules;

namespace StudioX.Zero.AspNetCore
{
    [DependsOn(typeof(StudioXZeroCoreModule))]
    public class StudioXZeroAspNetCoreModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXZeroAspNetCoreConfiguration, StudioXZeroAspNetCoreConfiguration>();
            Configuration.ReplaceService<IUserTokenProviderAccessor, AspNetCoreUserTokenProviderAccessor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}