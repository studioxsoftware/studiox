using StudioX.Configuration.Startup;

namespace StudioX.Quartz.Quartz.Configuration
{
    public static class StudioXQuartzConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure StudioX Quartz module.
        /// </summary>
        public static IStudioXQuartzConfiguration StudioXQuartz(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXQuartzConfiguration>();
        }
    }
}
