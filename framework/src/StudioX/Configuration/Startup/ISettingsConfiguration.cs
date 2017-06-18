using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     Used to configure setting system.
    /// </summary>
    public interface ISettingsConfiguration
    {
        /// <summary>
        ///     List of settings providers.
        /// </summary>
        ITypeList<SettingProvider> Providers { get; }
    }
}