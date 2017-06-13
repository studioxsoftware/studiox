using StudioX.Configuration.Startup;

namespace StudioX.EntityFrameworkCore.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX EntityFramework Core module.
    /// </summary>
    public static class StudioXEfCoreConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX EntityFramework Core module.
        /// </summary>
        public static IStudioXEfCoreConfiguration StudioXEfCore(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXEfCoreConfiguration>();
        }
    }
}