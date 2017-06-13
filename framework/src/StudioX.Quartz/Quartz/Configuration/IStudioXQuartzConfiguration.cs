using Quartz;

namespace StudioX.Quartz.Configuration
{
    public interface IStudioXQuartzConfiguration
    {
        IScheduler Scheduler { get;}
    }
}