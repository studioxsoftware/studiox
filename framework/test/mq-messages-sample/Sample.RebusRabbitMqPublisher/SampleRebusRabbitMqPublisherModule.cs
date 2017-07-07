using System.Reflection;
using Castle.Facilities.Logging;
using Rebus.NLog;
using Sample.BackgroundWorks;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.MqMessages.Publishers;
using StudioX.Threading.BackgroundWorkers;

namespace Sample
{
    [DependsOn(typeof(RebusRabbitMqPublisherModule))]
    public class SampleRebusRabbitMqPublisherModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.UseRebusRabbitMqPublisher()
                .UseLogging(c => c.NLog())
                .ConnectionTo("amqp://dev:dev@rabbitmq.local.cn/dev_host");

            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            StudioX.Dependency.IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseNLog().WithConfig("nlog.config"));

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<TestWorker>());
        }
    }
}