using StudioX;

namespace Sample
{
    public class SampleRebusRabbitMqConsumerBootstrap
    {
        private static readonly StudioXBootstrapper studioXBootstrapper =
            StudioXBootstrapper.Create<SampleRebusRabbitMqConsumerModule>();

        public void Start()
        {
            //LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            studioXBootstrapper.Initialize();
        }

        public void Stop()
        {
            studioXBootstrapper.Dispose();
        }
    }
}