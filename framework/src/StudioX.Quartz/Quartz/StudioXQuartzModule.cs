using System.Reflection;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Quartz.Configuration;
using StudioX.Threading.BackgroundWorkers;
using Quartz;

namespace StudioX.Quartz
{
    [DependsOn(typeof (StudioXKernelModule))]
    public class StudioXQuartzModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXQuartzConfiguration, StudioXQuartzConfiguration>();

            Configuration.Modules.StudioXQuartz().Scheduler.JobFactory = new StudioXQuartzJobFactory(IocManager);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IJobListener, StudioXQuartzJobListener>();

            Configuration.Modules.StudioXQuartz().Scheduler.ListenerManager.AddJobListener(IocManager.Resolve<IJobListener>());

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().Add(IocManager.Resolve<IQuartzScheduleJobManager>());
            }
        }
    }
}
