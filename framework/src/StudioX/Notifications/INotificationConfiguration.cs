using StudioX.Collections;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Used to configure notification system.
    /// </summary>
    public interface INotificationConfiguration
    {
        /// <summary>
        ///     Notification providers.
        /// </summary>
        ITypeList<NotificationProvider> Providers { get; }
    }
}