using Quartz;
using Quartz.Impl;

namespace StudioX.Quartz.Quartz.Configuration
{
    public class StudioXQuartzConfiguration : IStudioXQuartzConfiguration
    {
        public IScheduler Scheduler => StdSchedulerFactory.GetDefaultScheduler();
    }
}