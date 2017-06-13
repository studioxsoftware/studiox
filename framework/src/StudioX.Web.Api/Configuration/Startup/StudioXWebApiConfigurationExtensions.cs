using StudioX.WebApi.Configuration;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX.Web.Api module.
    /// </summary>
    public static class StudioXWebApiConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX.Web.Api module.
        /// </summary>
        public static IStudioXWebApiConfiguration StudioXWebApi(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXWebApiConfiguration>();
        }
    }
}