using System.Reflection;
using StudioX.Modules;

namespace StudioX.Quartz.Tests
{
    [DependsOn(typeof(StudioXQuartzModule))]
    public class StudioXQuartzTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
