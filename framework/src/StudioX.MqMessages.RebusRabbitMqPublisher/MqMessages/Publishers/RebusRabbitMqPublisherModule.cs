using System.Reflection;
using Rebus.Auditing.Messages;
using Rebus.Bus;
using Rebus.Config;
using StudioX.Modules;

namespace StudioX.MqMessages.Publishers
{
    public class RebusRabbitMqPublisherModule : StudioXModule
    {
        private IBus bus;

        public override void PreInitialize()
        {
            IocManager.Register<IRebusRabbitMqPublisherModuleConfig, RebusRabbitMqPublisherModuleConfig>();
            IocManager.Register<IMqMessagePublisher, RebusRabbitMqPublisher>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var moduleConfig = IocManager.Resolve<IRebusRabbitMqPublisherModuleConfig>();
            if (moduleConfig.Enabled)
            {
                var rebusConfig = Configure.With(new CastleWindsorContainerAdapter(IocManager.IocContainer));
                if (moduleConfig.MessageAuditingEnabled)
                {
                    rebusConfig.Options(o => o.EnableMessageAuditing(moduleConfig.MessageAuditingQueueName));
                }

                if (moduleConfig.LoggingConfigurer != null)
                {
                    rebusConfig.Logging(moduleConfig.LoggingConfigurer);
                }

                bus = rebusConfig.Transport(t => t.UseRabbitMqAsOneWayClient(moduleConfig.ConnectionString))
                    .Start();
            }
        }

        public override void Shutdown()
        {
            bus?.Dispose();
        }
    }
}