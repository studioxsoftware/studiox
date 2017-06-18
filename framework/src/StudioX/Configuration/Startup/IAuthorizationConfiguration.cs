using StudioX.Authorization;
using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     Used to configure authorization system.
    /// </summary>
    public interface IAuthorizationConfiguration
    {
        /// <summary>
        ///     List of authorization providers.
        /// </summary>
        ITypeList<AuthorizationProvider> Providers { get; }

        /// <summary>
        ///     Enables/Disables attribute based authentication and authorization.
        ///     Default: true.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}