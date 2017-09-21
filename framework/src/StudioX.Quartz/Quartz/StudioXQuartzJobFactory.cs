using StudioX.Dependency;
using StudioX.Extensions;
using Quartz;
using Quartz.Spi;

namespace StudioX.Quartz
{
    public class StudioXQuartzJobFactory : IJobFactory
    {
        private readonly IIocResolver _iocResolver;

        public StudioXQuartzJobFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            _iocResolver.Release(job);
        }
    }
}