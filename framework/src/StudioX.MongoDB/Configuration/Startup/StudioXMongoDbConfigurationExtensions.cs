using StudioX.MongoDb.Configuration;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX MongoDb module.
    /// </summary>
    public static class StudioXMongoDbConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX MongoDb module.
        /// </summary>
        public static IStudioXMongoDbModuleConfiguration StudioXMongoDb(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXMongoDbModuleConfiguration>();
        }
    }
}