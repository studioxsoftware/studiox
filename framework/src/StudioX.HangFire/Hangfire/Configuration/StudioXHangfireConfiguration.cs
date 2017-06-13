using Hangfire;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

namespace StudioX.Hangfire.Configuration
{
    public class StudioXHangfireConfiguration : IStudioXHangfireConfiguration
    {
        public BackgroundJobServer Server { get; set; }

        public IGlobalConfiguration GlobalConfiguration
        {
            get { return HangfireGlobalConfiguration.Configuration; }
        }
    }
}