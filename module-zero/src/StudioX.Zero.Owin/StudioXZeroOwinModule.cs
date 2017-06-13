using System.Reflection;
using StudioX.Authorization.Users;
using StudioX.Modules;
using StudioX.Zero;
using StudioX.Configuration.Startup;
using StudioX.Owin;

namespace StudioX
{
    [DependsOn(typeof(StudioXZeroCoreModule))]
    public class StudioXZeroOwinModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IUserTokenProviderAccessor, OwinUserTokenProviderAccessor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
