using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Json;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationSubscriptionManager" />.
    /// </summary>
    public class NotificationSubscriptionManager : INotificationSubscriptionManager, ITransientDependency
    {
        private readonly INotificationStore store;
        private readonly INotificationDefinitionManager notificationDefinitionManager;
        private readonly IGuidGenerator guidGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationSubscriptionManager" /> class.
        /// </summary>
        public NotificationSubscriptionManager(
            INotificationStore store,
            INotificationDefinitionManager notificationDefinitionManager,
            IGuidGenerator guidGenerator)
        {
            this.store = store;
            this.notificationDefinitionManager = notificationDefinitionManager;
            this.guidGenerator = guidGenerator;
        }

        public async Task SubscribeAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            if (await IsSubscribedAsync(user, notificationName, entityIdentifier))
            {
                return;
            }

            await store.InsertSubscriptionAsync(
                new NotificationSubscriptionInfo(
                    guidGenerator.Create(),
                    user.TenantId,
                    user.UserId,
                    notificationName,
                    entityIdentifier
                )
            );
        }

        public async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
        {
            var notificationDefinitions = (await notificationDefinitionManager
                    .GetAllAvailableAsync(user))
                .Where(nd => nd.EntityType == null)
                .ToList();

            foreach (var notificationDefinition in notificationDefinitions)
            {
                await SubscribeAsync(user, notificationDefinition.Name);
            }
        }

        public async Task UnsubscribeAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            await store.DeleteSubscriptionAsync(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }

        // TODO: Can work only for single database approach!
        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await store.GetSubscriptionsAsync(
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(int? tenantId, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await store.GetSubscriptionsAsync(
                new[] {tenantId},
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(UserIdentifier user)
        {
            var notificationSubscriptionInfos = await store.GetSubscriptionsAsync(user);

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            return store.IsSubscribedAsync(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }
    }
}