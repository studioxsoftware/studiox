using StudioX.MqMessages.Consumers;

namespace StudioX.Configuration.Startup
{
    public static class RebusRabbitMqConsumerConfigurationExtensions
    {
        public static IRebusRabbitMqConsumerModuleConfig UseRebusRabbitMqConsumer(
            this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.GetOrCreate("Modules.StudioX.RebusRabbitMqConsumer",
                () => configurations.StudioXConfiguration.IocManager.Resolve<IRebusRabbitMqConsumerModuleConfig>());
        }
    }
}