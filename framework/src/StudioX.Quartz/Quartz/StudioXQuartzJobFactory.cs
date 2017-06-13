using StudioX.Dependency;
using StudioX.Extensions;
using Quartz;
using Quartz.Spi;

namespace StudioX.Quartz
{
    public class StudioXQuartzJobFactory : IJobFactory
    {
        private readonly IIocResolver iocResolver;

        public StudioXQuartzJobFactory(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            iocResolver.Release(job);
        }
    }
}