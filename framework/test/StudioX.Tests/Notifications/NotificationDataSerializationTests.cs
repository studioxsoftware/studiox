using StudioX.Json;
using StudioX.Localization;
using StudioX.Notifications;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Notifications
{
    public class NotificationDataSerializationTests
    {
        [Fact]
        public void ShouldDeserializeAndSerializeMessageNotificationData()
        {
            var data = JsonConvert
                .DeserializeObject(
                    new MessageNotificationData("Hello World!").ToJsonString(),
                    typeof(MessageNotificationData)
                ) as MessageNotificationData;

            Assert.NotNull(data);
            data.Message.ShouldBe("Hello World!");
        }

        [Fact]
        public void ShouldDeserializeAndSerializeLocalizableMessageNotificationData()
        {
            var serialized = new LocalizableMessageNotificationData(new LocalizableString("Hello", "MySource")).ToJsonString();

            var data = JsonConvert
                .DeserializeObject(
                    serialized,
                    typeof(LocalizableMessageNotificationData)
                ) as LocalizableMessageNotificationData;

            Assert.NotNull(data);
            Assert.NotNull(data.Message);
            data.Message.Name.ShouldBe("Hello");
            data.Message.SourceName.ShouldBe("MySource");
        }

        [Fact]
        public void MessageNotificationDataBackwardCompatibilityTest()
        {
            const string serialized = "{\"Message\":\"a test message\",\"Type\":\"StudioX.Notifications.MessageNotificationData\",\"Properties\":{}}";

            var data = JsonConvert
                .DeserializeObject(
                    serialized,
                    typeof(MessageNotificationData)
                ) as MessageNotificationData;

            Assert.NotNull(data);
            data.Message.ShouldBe("a test message");
            data.Properties["Message"].ShouldBe("a test message");
        }
    }
}
