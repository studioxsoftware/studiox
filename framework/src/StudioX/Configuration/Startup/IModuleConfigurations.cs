namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// Used to provide a way to configure modules.
    /// Create entension methods to this class to be used over <see cref="IStudioXStartupConfiguration.Modules"/> object.
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        /// Gets the StudioX configuration object.
        /// </summary>
        IStudioXStartupConfiguration StudioXConfiguration { get; }
    }
}