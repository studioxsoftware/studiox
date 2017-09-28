using System;
using System.Threading.Tasks;
using StudioX.BackgroundJobs;
using StudioX.Hangfire.Configuration;
using StudioX.Threading.BackgroundWorkers;
using Hangfire;
using HangfireBackgroundJob = Hangfire.BackgroundJob;

namespace StudioX.Hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IStudioXHangfireConfiguration _hangfireConfiguration;

        public HangfireBackgroundJobManager(
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IStudioXHangfireConfiguration hangfireConfiguration)
        {
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _hangfireConfiguration = hangfireConfiguration;
        }

        public override void Start()
        {
            base.Start();

            if (_hangfireConfiguration.Server == null && _backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _hangfireConfiguration.Server = new BackgroundJobServer();
            }
        }

        public override void WaitToStop()
        {
            if (_hangfireConfiguration.Server != null)
            {
                try
                {
                    _hangfireConfiguration.Server.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            base.WaitToStop();
        }

        public Task EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJob<TArgs>
        {
            if (!delay.HasValue)
                HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            return Task.FromResult(0);
        }
    }
}
