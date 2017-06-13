using StudioX.Configuration.Startup;

namespace StudioX.Web.Mvc.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX.Web.Api module.
    /// </summary>
    public static class StudioXMvcConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX.Web.Api module.
        /// </summary>
        public static IStudioXMvcConfiguration StudioXMvc(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXMvcConfiguration>();
        }
    }
}