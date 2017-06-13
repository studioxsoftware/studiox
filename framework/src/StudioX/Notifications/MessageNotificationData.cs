using System;

namespace StudioX.Notifications
{
    /// <summary>
    /// Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class MessageNotificationData : NotificationData
    {
        /// <summary>
        /// The message.
        /// </summary>
        public string Message
        {
            get => message ?? (this[nameof(Message)] as string);
            set
            {
                this[nameof(Message)] = value;
                message = value;
            }
        }
        private string message;

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private MessageNotificationData()
        {
            
        }

        public MessageNotificationData(string message)
        {
            Message = message;
        }
    }
}