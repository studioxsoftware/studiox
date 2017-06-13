using System;
using System.Threading.Tasks;
using StudioX.BackgroundJobs;
using StudioX.Dependency;
using StudioX.Quartz.Configuration;
using StudioX.Threading.BackgroundWorkers;
using Quartz;

namespace StudioX.Quartz
{
    public class QuartzScheduleJobManager : BackgroundWorkerBase, IQuartzScheduleJobManager, ISingletonDependency
    {
        private readonly IBackgroundJobConfiguration backgroundJobConfiguration;
        private readonly IStudioXQuartzConfiguration quartzConfiguration;

        public QuartzScheduleJobManager(
            IStudioXQuartzConfiguration quartzConfiguration,
            IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            this.quartzConfiguration = quartzConfiguration;
            this.backgroundJobConfiguration = backgroundJobConfiguration;
        }

        public Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger)
            where TJob : IJob
        {
            var jobToBuild = JobBuilder.Create<TJob>();
            configureJob(jobToBuild);
            var job = jobToBuild.Build();

            var triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            var trigger = triggerToBuild.Build();

            quartzConfiguration.Scheduler.ScheduleJob(job, trigger);

            return Task.FromResult(0);
        }

        public override void Start()
        {
            base.Start();

            if (backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                quartzConfiguration.Scheduler.Start();
            }

            Logger.Info("Started QuartzScheduleJobManager");
        }

        public override void WaitToStop()
        {
            if (quartzConfiguration.Scheduler != null)
            {
                try
                {
                    quartzConfiguration.Scheduler.Shutdown(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            base.WaitToStop();

            Logger.Info("Stopped QuartzScheduleJobManager");
        }
    }
}