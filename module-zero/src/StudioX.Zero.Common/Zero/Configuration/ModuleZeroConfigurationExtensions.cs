using StudioX.Configuration.Startup;

namespace StudioX.Zero.Configuration
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroConfigurationExtensions
    {
        /// <summary>
        /// Used to configure module zero.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static IStudioXZeroConfig Zero(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.StudioXConfiguration.Get<IStudioXZeroConfig>();
        }
    }
}