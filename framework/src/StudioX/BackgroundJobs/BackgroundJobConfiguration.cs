using StudioX.Configuration.Startup;

namespace StudioX.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public bool IsJobExecutionEnabled { get; set; }
        
        public IStudioXStartupConfiguration StudioXConfiguration { get; private set; }

        public BackgroundJobConfiguration(IStudioXStartupConfiguration startupConfiguration)
        {
            StudioXConfiguration = startupConfiguration;
            IsJobExecutionEnabled = true;
        }
    }
}