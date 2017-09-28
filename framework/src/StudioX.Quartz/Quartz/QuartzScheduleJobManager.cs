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
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IStudioXQuartzConfiguration _quartzConfiguration;

        public QuartzScheduleJobManager(
            IStudioXQuartzConfiguration quartzConfiguration,
            IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            _quartzConfiguration = quartzConfiguration;
            _backgroundJobConfiguration = backgroundJobConfiguration;
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

            _quartzConfiguration.Scheduler.ScheduleJob(job, trigger);

            return Task.FromResult(0);
        }

        public override void Start()
        {
            base.Start();

            if (_backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _quartzConfiguration.Scheduler.Start();
            }

            Logger.Info("Started QuartzScheduleJobManager");
        }

        public override void WaitToStop()
        {
            if (_quartzConfiguration.Scheduler != null)
            {
                try
                {
                    _quartzConfiguration.Scheduler.Shutdown(true);
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