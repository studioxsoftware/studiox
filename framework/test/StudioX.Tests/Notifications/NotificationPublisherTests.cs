using System.Threading.Tasks;
using StudioX.BackgroundJobs;
using StudioX.Domain.Uow;
using StudioX.Notifications;
using NSubstitute;
using Xunit;

namespace StudioX.Tests.Notifications
{
    public class NotificationPublisherTests : TestBaseWithLocalIocManager
    {
        private readonly NotificationPublisher publisher;
        private readonly INotificationStore store;
        private readonly IBackgroundJobManager backgroundJobManager;

        public NotificationPublisherTests()
        {
            store = Substitute.For<INotificationStore>();
            backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            publisher = new NotificationPublisher(store, backgroundJobManager, Substitute.For<INotificationDistributer>(), SequentialGuidGenerator.Instance);
            publisher.UnitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            publisher.UnitOfWorkManager.Current.Returns(Substitute.For<IActiveUnitOfWork>());
        }

        [Fact]
        public async Task ShouldPublishGeneralNotification()
        {
            //Arrange
            var notificationData = CreateNotificationData();

            //Act
            await publisher.PublishAsync("TestNotification", notificationData, severity: NotificationSeverity.Success);

            //Assert
            await store.Received()
                .InsertNotificationAsync(
                    Arg.Is<NotificationInfo>(
                        n => n.NotificationName == "TestNotification" &&
                             n.Severity == NotificationSeverity.Success &&
                             n.DataTypeName == notificationData.GetType().AssemblyQualifiedName &&
                             n.Data.Contains("42")
                        )
                );

            await backgroundJobManager.Received()
                .EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                    Arg.Any<NotificationDistributionJobArgs>()
                );
        }

        private static NotificationData CreateNotificationData()
        {
            var notificationData = new NotificationData();
            notificationData["TestValue"] = 42;
            return notificationData;
        }
    }
}
