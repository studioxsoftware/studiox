using System.Reflection;
using Castle.Facilities.Logging;
using Rebus.NLog;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.MqMessages.Consumers;

namespace Sample
{
    [DependsOn(typeof(RebusRabbitMqConsumerModule))]
    public class SampleRebusRabbitMqConsumerModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.UseRebusRabbitMqConsumer()
                .UseLogging(c => c.NLog())
                .ConnectTo("amqp://dev:dev@rabbitmq.local.cn/dev_host")
                .UseQueue(Assembly.GetExecutingAssembly().GetName().Name)
                .RegisterHandlerInAssemblys(Assembly.GetExecutingAssembly());
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void PostInitialize()
        {
            StudioX.Dependency.IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseNLog().WithConfig("nlog.config"));
        }
    }
}