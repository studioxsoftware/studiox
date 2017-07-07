using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Rebus.Bus;
using StudioX.Json;
using StudioX.Runtime.Session;
using StudioX.Threading;

namespace StudioX.MqMessages.Publishers
{
    public class RebusRabbitMqPublisher : IMqMessagePublisher
    {
        private readonly IBus bus;

        public ILogger Logger { get; set; }

        public IStudioXSession StudioXSession { get; set; }

        public RebusRabbitMqPublisher(IBus bus)
        {
            this.bus = bus;
            Logger = NullLogger.Instance;
            StudioXSession = NullStudioXSession.Instance;
        }

        public void Publish(object mqMessages)
        {
            TryFillSessionInfo(mqMessages);

            Logger.Debug(mqMessages.GetType().FullName + ":" + mqMessages.ToJsonString());

            AsyncHelper.RunSync(() => bus.Publish(mqMessages));
        }

        private void TryFillSessionInfo(object mqMessages)
        {
            if (StudioXSession.UserId.HasValue)
            {
                var operatorUserIdProperty = mqMessages.GetType().GetProperty("UserId");
                if (operatorUserIdProperty != null && operatorUserIdProperty.PropertyType == typeof(long?))
                {
                    operatorUserIdProperty.SetValue(mqMessages, StudioXSession.UserId);
                }
            }

            if (StudioXSession.TenantId.HasValue)
            {
                var tenantIdProperty = mqMessages.GetType().GetProperty("TenantId");
                if (tenantIdProperty != null && tenantIdProperty.PropertyType == typeof(int?))
                {
                    tenantIdProperty.SetValue(mqMessages, StudioXSession.TenantId);
                }
            }
        }

        public async Task PublishAsync(object mqMessages)
        {
            TryFillSessionInfo(mqMessages);

            Logger.Debug(mqMessages.GetType().FullName + ":" + mqMessages.ToJsonString());

            await bus.Publish(mqMessages);
        }
    }
}