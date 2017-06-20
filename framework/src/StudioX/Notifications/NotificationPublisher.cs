using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.BackgroundJobs;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.Json;
using StudioX.Runtime.Session;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationPublisher" />.
    /// </summary>
    public class NotificationPublisher : StudioXServiceBase, INotificationPublisher, ITransientDependency
    {
        public const int MaxUserCountToDirectlyDistributeANotification = 5;

        /// <summary>
        ///     Indicates all tenants.
        /// </summary>
        public static int[] AllTenants
        {
            get { return new[] {NotificationInfo.AllTenantIds.To<int>()}; }
        }

        /// <summary>
        ///     Reference to StudioX session.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        private readonly INotificationStore store;
        private readonly IBackgroundJobManager backgroundJobManager;
        private readonly INotificationDistributer notificationDistributer;
        private readonly IGuidGenerator guidGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationPublisher" /> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager,
            INotificationDistributer notificationDistributer,
            IGuidGenerator guidGenerator)
        {
            this.store = store;
            this.backgroundJobManager = backgroundJobManager;
            this.notificationDistributer = notificationDistributer;
            this.guidGenerator = guidGenerator;
            StudioXSession = NullStudioXSession.Instance;
        }

        //Create EntityIdentifier includes entityType and entityId.
        [UnitOfWork]
        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] tenantIds = null)
        {
            if (notificationName.IsNullOrEmpty())
            {
                throw new ArgumentException("NotificationName can not be null or whitespace!", "notificationName");
            }

            if (!tenantIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
            {
                throw new ArgumentException("tenantIds can be set only if userIds is not set!", "tenantIds");
            }

            if (tenantIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
            {
                tenantIds = new[] {StudioXSession.TenantId};
            }

            var notificationInfo = new NotificationInfo(guidGenerator.Create())
            {
                NotificationName = notificationName,
                EntityTypeName = entityIdentifier?.Type.FullName,
                EntityTypeAssemblyQualifiedName = entityIdentifier?.Type.AssemblyQualifiedName,
                EntityId = entityIdentifier?.Id.ToJsonString(),
                Severity = severity,
                UserIds =
                    userIds.IsNullOrEmpty()
                        ? null
                        : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                ExcludedUserIds =
                    excludedUserIds.IsNullOrEmpty()
                        ? null
                        : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                TenantIds = tenantIds.IsNullOrEmpty() ? null : tenantIds.JoinAsString(","),
                Data = data?.ToJsonString(),
                DataTypeName = data?.GetType().AssemblyQualifiedName
            };

            await store.InsertNotificationAsync(notificationInfo);

            await CurrentUnitOfWork.SaveChangesAsync(); //To get Id of the notification

            if (userIds != null && userIds.Length <= MaxUserCountToDirectlyDistributeANotification)
            {
                //We can directly distribute the notification since there are not much receivers
                await notificationDistributer.DistributeAsync(notificationInfo.Id);
            }
            else
            {
                //We enqueue a background job since distributing may get a long time
                await backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                    new NotificationDistributionJobArgs(
                        notificationInfo.Id
                    )
                );
            }
        }
    }
}