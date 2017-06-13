using System;
using Xunit;

namespace StudioX.Tests.Events.Bus
{
    public class ActionBasedEventHandlerTest : EventBusTestBase
    {
        [Fact]
        public void ShouldCallActionOnEventWithCorrectSource()
        {
            var totalData = 0;

            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.Equal(this, eventData.EventSource);
                });

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));
            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.Equal(10, totalData);
        }

        [Fact]
        public void ShouldCallHandlerWithNonGenericTrigger()
        {
            var totalData = 0;

            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.Equal(this, eventData.EventSource);
                });

            EventBus.Trigger(typeof(MySimpleEventData), this, new MySimpleEventData(1));
            EventBus.Trigger(typeof(MySimpleEventData), this, new MySimpleEventData(2));
            EventBus.Trigger(typeof(MySimpleEventData), this, new MySimpleEventData(3));
            EventBus.Trigger(typeof(MySimpleEventData), this, new MySimpleEventData(4));

            Assert.Equal(10, totalData);
        }

        [Fact]
        public void ShouldNotCallActionAfterUnregister1()
        {
            var totalData = 0;

            var registerDisposer = EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                });

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));

            registerDisposer.Dispose();

            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.Equal(6, totalData);
        }

        [Fact]
        public void ShouldNotCallActionAfterUnregister2()
        {
            var totalData = 0;

            var action = new Action<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                });

            EventBus.Register(action);

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));

            EventBus.Unregister(action);

            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.Equal(6, totalData);
        }
    }
}
