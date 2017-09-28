using System;
using System.Threading.Tasks;
using StudioX.Domain.Services;

namespace StudioX.Notifications
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public interface INotificationDistributer : IDomainService
    {
        /// <summary>
        /// Distributes given notification to users.
        /// </summary>
        /// <param name="notificationId">The notification id.</param>
        Task DistributeAsync(Guid notificationId);
    }
}