using StudioX.Collections;

namespace StudioX.Notifications
{
    internal class NotificationConfiguration : INotificationConfiguration
    {
        public ITypeList<NotificationProvider> Providers { get; }

        public NotificationConfiguration()
        {
            Providers = new TypeList<NotificationProvider>();
        }
    }
}