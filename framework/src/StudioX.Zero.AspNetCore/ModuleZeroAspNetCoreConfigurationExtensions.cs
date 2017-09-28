using StudioX.Configuration.Startup;

namespace StudioX.Zero.AspNetCore
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroAspNetCoreConfigurationExtensions
    {
        /// <summary>
        /// Configures StudioX Zero AspNetCore module.
        /// </summary>
        /// <returns></returns>
        public static IStudioXZeroAspNetCoreConfiguration ZeroAspNetCore(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.StudioXConfiguration.Get<IStudioXZeroAspNetCoreConfiguration>();
        }
    }
}