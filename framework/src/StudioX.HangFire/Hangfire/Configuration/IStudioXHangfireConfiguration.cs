using StudioX.BackgroundJobs;
using StudioX.Modules;
using Hangfire;

namespace StudioX.Hangfire.Configuration
{
    /// <summary>
    /// Used to configure Hangfire.
    /// </summary>
    public interface IStudioXHangfireConfiguration
    {
        /// <summary>
        /// Gets or sets the Hanfgire's <see cref="BackgroundJobServer"/> object.
        /// Important: This is null in <see cref="StudioXModule.PreInitialize"/>. You can create and set it to customize it's creation.
        /// If you don't set it, it's automatically set in <see cref="StudioXModule.PreInitialize"/> by StudioX.HangFire module with it's default constructor
        /// if background job execution is enabled (see <see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>).
        /// So, if you create it yourself, it's your responsibility to check if background job execution is enabled (see <see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>).
        /// </summary>
        BackgroundJobServer Server { get; set; }

        /// <summary>
        /// A reference to Hangfire's global configuration.
        /// </summary>
        IGlobalConfiguration GlobalConfiguration { get; }
    }
}