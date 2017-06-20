namespace StudioX.Notifications
{
    internal class NotificationDefinitionContext : INotificationDefinitionContext
    {
        public INotificationDefinitionManager Manager { get; }

        public NotificationDefinitionContext(INotificationDefinitionManager manager)
        {
            Manager = manager;
        }
    }
}