using StudioX.MemoryDb.Configuration;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX MemoryDb module.
    /// </summary>
    public static class StudioXMemoryDbConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX MemoryDb module.
        /// </summary>
        public static IStudioXMemoryDbModuleConfiguration StudioXMemoryDb(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXMemoryDbModuleConfiguration>();
        }
    }
}