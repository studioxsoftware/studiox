using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.TestBase
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXTestBaseModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.EventBus.UseDefaultEventBus = false;
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXTestBaseModule).GetAssembly());
        }
    }
}