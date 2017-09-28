using StudioX.Configuration.Startup;

namespace StudioX.AutoMapper
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX.AutoMapper module.
    /// </summary>
    public static class StudioXAutoMapperConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX.AutoMapper module.
        /// </summary>
        public static IStudioXAutoMapperConfiguration StudioXAutoMapper(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXAutoMapperConfiguration>();
        }
    }
}