using Microsoft.AspNet.Identity;

namespace StudioX.Zero.AspNetCore
{
    public interface IStudioXZeroAspNetCoreConfiguration
    {
        /// <summary>
        /// Authentication scheme of the application.
        /// </summary>
        string AuthenticationScheme { get; set; }

        /// <summary>
        /// Default value: <see cref="DefaultAuthenticationTypes.TwoFactorCookie"/>.
        /// </summary>
        string TwoFactorAuthenticationScheme { get; set; }

        /// <summary>
        /// Default value: <see cref="DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie"/>.
        /// </summary>
        string TwoFactorRememberBrowserAuthenticationScheme { get; set; }
    }
}