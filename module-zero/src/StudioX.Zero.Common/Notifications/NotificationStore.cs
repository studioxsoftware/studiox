using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Linq.Extensions;

namespace StudioX.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationStore"/> using repositories.
    /// </summary>
    public class NotificationStore : INotificationStore, ITransientDependency
    {
        private readonly IRepository<NotificationInfo, Guid> notificationRepository;
        private readonly IRepository<TenantNotificationInfo, Guid> tenantNotificationRepository;
        private readonly IRepository<UserNotificationInfo, Guid> userNotificationRepository;
        private readonly IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationStore"/> class.
        /// </summary>
        public NotificationStore(
            IRepository<NotificationInfo, Guid> notificationRepository,
            IRepository<TenantNotificationInfo, Guid> tenantNotificationRepository,
            IRepository<UserNotificationInfo, Guid> userNotificationRepository,
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.notificationRepository = notificationRepository;
            this.tenantNotificationRepository = tenantNotificationRepository;
            this.userNotificationRepository = userNotificationRepository;
            this.notificationSubscriptionRepository = notificationSubscriptionRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            using (unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
            {
                await notificationSubscriptionRepository.InsertAsync(subscription);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                await notificationSubscriptionRepository.DeleteAsync(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task InsertNotificationAsync(NotificationInfo notification)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                await notificationRepository.InsertAsync(notification);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                return await notificationRepository.FirstOrDefaultAsync(notificationId);
            }
        }

        [UnitOfWork]
        public virtual async Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            using (unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
            {
                await userNotificationRepository.InsertAsync(userNotification);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId)
        {
            using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
            {
                return notificationSubscriptionRepository.GetAllListAsync(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
            }
        }

        [UnitOfWork]
        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] tenantIds, string notificationName, string entityTypeName, string entityId)
        {
            var subscriptions = new List<NotificationSubscriptionInfo>();

            foreach (var tenantId in tenantIds)
            {
                subscriptions.AddRange(await GetSubscriptionsAsync(tenantId, notificationName, entityTypeName, entityId));
            }

            return subscriptions;
        }

        [UnitOfWork]
        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return await notificationSubscriptionRepository.GetAllListAsync(s => s.UserId == user.UserId);
            }
        }
        
        [UnitOfWork]
        protected virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int? tenantId, string notificationName, string entityTypeName, string entityId)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await notificationSubscriptionRepository.GetAllListAsync(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                );
            }
        }

        [UnitOfWork]
        public virtual async Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return await notificationSubscriptionRepository.CountAsync(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    ) > 0;
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var userNotification = await userNotificationRepository.FirstOrDefaultAsync(userNotificationId);
                if (userNotification == null)
                {
                    return;
                }

                userNotification.State = state;
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var userNotifications = await userNotificationRepository.GetAllListAsync(un => un.UserId == user.UserId);

                foreach (var userNotification in userNotifications)
                {
                    userNotification.State = state;
                }

                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await userNotificationRepository.DeleteAsync(userNotificationId);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteAllUserNotificationsAsync(UserIdentifier user)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                await userNotificationRepository.DeleteAsync(un => un.UserId == user.UserId);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var query = from userNotificationInfo in userNotificationRepository.GetAll()
                            join tenantNotificationInfo in tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                            where userNotificationInfo.UserId == user.UserId && (state == null || userNotificationInfo.State == state.Value)
                            orderby tenantNotificationInfo.CreationTime descending
                            select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                query = query.PageBy(skipCount, maxResultCount);

                var list = query.ToList();

                return Task.FromResult(list.Select(
                    a => new UserNotificationInfoWithNotificationInfo(a.userNotificationInfo, a.tenantNotificationInfo)
                    ).ToList());
            }
        }

        [UnitOfWork]
        public virtual async Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return await userNotificationRepository.CountAsync(un => un.UserId == user.UserId && (state == null || un.State == state.Value));
            }
        }

        [UnitOfWork]
        public virtual Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(int? tenantId, Guid userNotificationId)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userNotificationInfo in userNotificationRepository.GetAll()
                            join tenantNotificationInfo in tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                            where userNotificationInfo.Id == userNotificationId
                            select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                var item = query.FirstOrDefault();
                if (item == null)
                {
                    return Task.FromResult((UserNotificationInfoWithNotificationInfo)null);
                }

                return Task.FromResult(new UserNotificationInfoWithNotificationInfo(item.userNotificationInfo, item.tenantNotificationInfo));
            }
        }

        [UnitOfWork]
        public virtual async Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
            {
                await tenantNotificationRepository.InsertAsync(tenantNotificationInfo);
            }
        }

        public virtual Task DeleteNotificationAsync(NotificationInfo notification)
        {
            return notificationRepository.DeleteAsync(notification);
        }
    }
}
