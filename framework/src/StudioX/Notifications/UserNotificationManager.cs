using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Dependency;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Implements  <see cref="IUserNotificationManager" />.
    /// </summary>
    public class UserNotificationManager : IUserNotificationManager, ISingletonDependency
    {
        private readonly INotificationStore store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotificationManager" /> class.
        /// </summary>
        public UserNotificationManager(INotificationStore store)
        {
            this.store = store;
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier user,
            UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            var userNotifications = await store.GetUserNotificationsWithNotificationsAsync(user, state, skipCount,
                maxResultCount);
            return userNotifications
                .Select(un => un.ToUserNotification())
                .ToList();
        }

        public Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null)
        {
            return store.GetUserNotificationCountAsync(user, state);
        }

        public async Task<UserNotification> GetUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            var userNotification = await store.GetUserNotificationWithNotificationOrNullAsync(
                tenantId, userNotificationId
            );
            return userNotification?.ToUserNotification();
        }

        public Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            return store.UpdateUserNotificationStateAsync(tenantId, userNotificationId, state);
        }

        public Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            return store.UpdateAllUserNotificationStatesAsync(user, state);
        }

        public Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            return store.DeleteUserNotificationAsync(tenantId, userNotificationId);
        }

        public Task DeleteAllUserNotificationsAsync(UserIdentifier user)
        {
            return store.DeleteAllUserNotificationsAsync(user);
        }
    }
}