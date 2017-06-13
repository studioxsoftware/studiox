using System;
using StudioX.BackgroundJobs;
using StudioX.Configuration.Startup;

namespace StudioX.Hangfire.Configuration
{
    public static class StudioXHangfireConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX Hangfire module.
        /// </summary>
        public static IStudioXHangfireConfiguration StudioXHangfire(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXHangfireConfiguration>();
        }

        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration, Action<IStudioXHangfireConfiguration> configureAction)
        {
            backgroundJobConfiguration.StudioXConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
            configureAction(backgroundJobConfiguration.StudioXConfiguration.Modules.StudioXHangfire());
        }
    }
}