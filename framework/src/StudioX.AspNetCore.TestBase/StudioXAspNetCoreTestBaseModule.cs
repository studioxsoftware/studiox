using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.TestBase;

namespace StudioX.AspNetCore.TestBase
{
    [DependsOn(typeof(StudioXTestBaseModule),typeof(StudioXAspNetCoreModule))]
    public class StudioXAspNetCoreTestBaseModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXAspNetCoreTestBaseModule).GetAssembly());
        }
    }
}