using Quartz;
using Quartz.Impl;

namespace StudioX.Quartz.Configuration
{
    public class StudioXQuartzConfiguration : IStudioXQuartzConfiguration
    {
        public IScheduler Scheduler => StdSchedulerFactory.GetDefaultScheduler();
    }
}