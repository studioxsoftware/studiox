using Quartz;

namespace StudioX.Quartz.Quartz.Configuration
{
    public interface IStudioXQuartzConfiguration
    {
        IScheduler Scheduler { get;}
    }
}