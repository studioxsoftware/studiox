using StudioX.Application.Navigation;
using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     Used to configure navigation.
    /// </summary>
    public interface INavigationConfiguration
    {
        /// <summary>
        ///     List of navigation providers.
        /// </summary>
        ITypeList<NavigationProvider> Providers { get; }
    }
}