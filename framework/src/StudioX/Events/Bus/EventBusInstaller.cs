using System.Reflection;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Events.Bus.Factories;
using StudioX.Events.Bus.Handlers;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace StudioX.Events.Bus
{
    /// <summary>
    /// Installs event bus system and registers all handlers automatically.
    /// </summary>
    internal class EventBusInstaller : IWindsorInstaller
    {
        private readonly IIocResolver iocResolver;
        private readonly IEventBusConfiguration eventBusConfiguration;
        private IEventBus eventBus;

        public EventBusInstaller(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            eventBusConfiguration = iocResolver.Resolve<IEventBusConfiguration>();
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                eventBusConfiguration.UseDefaultEventBus
                    ? Component.For<IEventBus>().UsingFactoryMethod(() => EventBus.Default).LifestyleSingleton()
                    : Component.For<IEventBus>().ImplementedBy<EventBus>().LifestyleSingleton()
            );

            eventBus = container.Resolve<IEventBus>();

            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            /* This code checks if registering component implements any IEventHandler<TEventData> interface, if yes,
             * gets all event handler interfaces and registers type to Event Bus for each handling event.
             */
            if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                return;
            }

            var interfaces = handler.ComponentModel.Implementation.GetTypeInfo().GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                {
                    continue;
                }

                var genericArgs = @interface.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    eventBus.Register(genericArgs[0], new IocHandlerFactory(iocResolver, handler.ComponentModel.Implementation));
                }
            }
        }
    }
}
