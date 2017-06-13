using StudioX.Configuration.Startup;

namespace StudioX.AspNetCore.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX ASP.NET Core module.
    /// </summary>
    public static class StudioXAspNetCoreConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX ASP.NET Core module.
        /// </summary>
        public static IStudioXAspNetCoreConfiguration StudioXAspNetCore(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXAspNetCoreConfiguration>();
        }
    }
}