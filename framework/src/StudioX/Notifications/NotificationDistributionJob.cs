using StudioX.BackgroundJobs;
using StudioX.Dependency;
using StudioX.Threading;

namespace StudioX.Notifications
{
    /// <summary>
    ///     This background job distributes notifications to users.
    /// </summary>
    public class NotificationDistributionJob : BackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        private readonly INotificationDistributer notificationDistributer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationDistributionJob" /> class.
        /// </summary>
        public NotificationDistributionJob(INotificationDistributer notificationDistributer)
        {
            this.notificationDistributer = notificationDistributer;
        }

        public override void Execute(NotificationDistributionJobArgs args)
        {
            AsyncHelper.RunSync(() => notificationDistributer.DistributeAsync(args.NotificationId));
        }
    }
}