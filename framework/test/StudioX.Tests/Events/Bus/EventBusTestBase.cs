using StudioX.Events.Bus;

namespace StudioX.Tests.Events.Bus
{
    public abstract class EventBusTestBase
    {
        protected IEventBus EventBus;

        protected EventBusTestBase()
        {
            EventBus = new EventBus();
        }
    }
}