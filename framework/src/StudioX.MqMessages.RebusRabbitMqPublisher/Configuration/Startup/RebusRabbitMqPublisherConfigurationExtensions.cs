using StudioX.MqMessages.Publishers;

namespace StudioX.Configuration.Startup
{
    public static class RebusRabbitMqPublisherConfigurationExtensions
    {
        public static IRebusRabbitMqPublisherModuleConfig UseRebusRabbitMqPublisher(
            this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.GetOrCreate("Modules.StudioX.RebusRabbitMqPublisher",
                () => configurations.StudioXConfiguration.IocManager.Resolve<IRebusRabbitMqPublisherModuleConfig>());
        }
    }
}