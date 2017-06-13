using StudioX.Web.Configuration;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX Web module.
    /// </summary>
    public static class StudioXWebConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX Web Common module.
        /// </summary>
        public static IStudioXWebCommonModuleConfiguration StudioXWebCommon(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXWebCommonModuleConfiguration>();
        }
    }
}