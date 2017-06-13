using StudioX.BackgroundJobs;
using StudioX.Configuration.Startup;

namespace StudioX.Hangfire.Configuration
{
    public static class StudioXHangfireConfigurationExtensions
    {
        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            backgroundJobConfiguration.StudioXConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
        }
    }
}