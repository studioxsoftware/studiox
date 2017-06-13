using System;

namespace StudioX.Events.Bus.Factories.Internals
{
    /// <summary>
    /// Used to unregister a <see cref="IEventHandlerFactory"/> on <see cref="Dispose"/> method.
    /// </summary>
    internal class FactoryUnregistrar : IDisposable
    {
        private readonly IEventBus eventBus;
        private readonly Type eventType;
        private readonly IEventHandlerFactory factory;

        public FactoryUnregistrar(IEventBus eventBus, Type eventType, IEventHandlerFactory factory)
        {
            this.eventBus = eventBus;
            this.eventType = eventType;
            this.factory = factory;
        }

        public void Dispose()
        {
            eventBus.Unregister(eventType, factory);
        }
    }
}