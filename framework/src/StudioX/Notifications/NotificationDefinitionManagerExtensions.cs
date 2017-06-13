using System.Collections.Generic;
using StudioX.Threading;

namespace StudioX.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="INotificationDefinitionManager"/>.
    /// </summary>
    public static class NotificationDefinitionManagerExtensions
    {
        /// <summary>
        /// Gets all available notification definitions for given user.
        /// </summary>
        /// <param name="notificationDefinitionManager">Notification definition manager</param>
        /// <param name="user">User</param>
        public static IReadOnlyList<NotificationDefinition> GetAllAvailable(this INotificationDefinitionManager notificationDefinitionManager, UserIdentifier user)
        {
            return AsyncHelper.RunSync(() => notificationDefinitionManager.GetAllAvailableAsync(user));
        }
    }
}