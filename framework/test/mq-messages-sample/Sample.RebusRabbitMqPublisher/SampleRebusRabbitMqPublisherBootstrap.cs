using StudioX;

namespace Sample
{
    public class SampleRebusRabbitMqPublisherBootstrap
    {
        private static readonly StudioXBootstrapper studioXBootstrapper =
            StudioXBootstrapper.Create<SampleRebusRabbitMqPublisherModule>();

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