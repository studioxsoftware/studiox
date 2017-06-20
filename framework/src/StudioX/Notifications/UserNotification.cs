using System;
using StudioX.Application.Services.Dto;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Represents a notification sent to a user.
    /// </summary>
    [Serializable]
    public class UserNotification : EntityDto<Guid>, IUserIdentifier
    {
        /// <summary>
        ///     TenantId.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        ///     User Id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        ///     The notification.
        /// </summary>
        public TenantNotification Notification { get; set; }
    }
}