using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.Hangfire
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXHangfireAspNetCoreModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXHangfireAspNetCoreModule).GetAssembly());
        }
    }
}
