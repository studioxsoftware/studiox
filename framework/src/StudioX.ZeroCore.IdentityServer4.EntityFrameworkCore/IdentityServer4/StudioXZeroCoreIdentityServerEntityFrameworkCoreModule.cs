using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero.EntityFrameworkCore;

namespace StudioX.IdentityServer4
{
    [DependsOn(typeof(StudioXZeroCoreIdentityServerModule), typeof(StudioXZeroCoreEntityFrameworkCoreModule))]
    public class StudioXZeroCoreIdentityServerEntityFrameworkCoreModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroCoreIdentityServerEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
