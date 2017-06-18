using StudioX.Configuration.Startup;

namespace StudioX.BackgroundJobs
{
    /// <summary>
    ///     Used to configure background job system.
    /// </summary>
    public interface IBackgroundJobConfiguration
    {
        /// <summary>
        ///     Used to enable/disable background job execution.
        /// </summary>
        bool IsJobExecutionEnabled { get; set; }

        /// <summary>
        ///     Gets the StudioX configuration object.
        /// </summary>
        IStudioXStartupConfiguration StudioXConfiguration { get; }
    }
}