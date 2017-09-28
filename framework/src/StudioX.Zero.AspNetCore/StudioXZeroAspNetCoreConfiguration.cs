using Microsoft.AspNet.Identity;

namespace StudioX.Zero.AspNetCore
{
    internal class StudioXZeroAspNetCoreConfiguration : IStudioXZeroAspNetCoreConfiguration
    {
        public string AuthenticationScheme { get; set; }

        public string TwoFactorAuthenticationScheme { get; set; }

        public string TwoFactorRememberBrowserAuthenticationScheme { get; set; }

        public StudioXZeroAspNetCoreConfiguration()
        {
            AuthenticationScheme = "AppAuthenticationScheme";
            TwoFactorAuthenticationScheme = DefaultAuthenticationTypes.TwoFactorCookie;
            TwoFactorRememberBrowserAuthenticationScheme = DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie;
        }
    }
}
