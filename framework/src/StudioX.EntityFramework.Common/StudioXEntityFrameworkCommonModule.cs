using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.EntityFramework
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXEntityFrameworkCommonModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXEntityFrameworkCommonModule).GetAssembly());
        }
    }
}
