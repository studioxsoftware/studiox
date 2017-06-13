using StudioX.Domain.Entities;
using StudioX.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Events.Bus
{
    public class GenericInheritanceTest : EventBusTestBase
    {
        [Fact]
        public void ShouldTriggerForInheritedGeneric1()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(42);
                    triggeredEvent = true;
                });

            EventBus.Trigger(new EntityUpdatedEventData<Person>(new Person { Id = 42 }));

            triggeredEvent.ShouldBe(true);
        }

        [Fact]
        public void ShouldTriggerForInheritedGeneric2()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(42);
                    triggeredEvent = true;
                });

            EventBus.Trigger(new EntityChangedEventData<Student>(new Student { Id = 42 }));

            triggeredEvent.ShouldBe(true);
        }
        
        
        public class Person : Entity
        {
            
        }

        public class Student : Person
        {
            
        }
    }
}