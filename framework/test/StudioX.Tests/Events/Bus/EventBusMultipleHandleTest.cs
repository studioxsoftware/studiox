using StudioX.Domain.Entities;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Events.Bus
{
    public class EventBusEntityEventsTest : EventBusTestBase
    {
        [Fact]
        public void ShouldCallCreatedAndChangedOnce()
        {
            var handler = new MyEventHandler();

            EventBus.Register<EntityChangedEventData<MyEntity>>(handler);
            EventBus.Register<EntityCreatedEventData<MyEntity>>(handler);

            EventBus.Trigger(new EntityCreatedEventData<MyEntity>(new MyEntity()));

            handler.EntityCreatedEventCount.ShouldBe(1);
            handler.EntityChangedEventCount.ShouldBe(1);
        }

        public class MyEntity : Entity
        {
            
        }

        public class MyEventHandler : 
            IEventHandler<EntityChangedEventData<MyEntity>>,
            IEventHandler<EntityCreatedEventData<MyEntity>>
        {
            public int EntityChangedEventCount { get; set; }
            public int EntityCreatedEventCount { get; set; }

            public void HandleEvent(EntityChangedEventData<MyEntity> eventData)
            {
                EntityChangedEventCount++;
            }

            public void HandleEvent(EntityCreatedEventData<MyEntity> eventData)
            {
                EntityCreatedEventCount++;
            }
        }
    }
}
