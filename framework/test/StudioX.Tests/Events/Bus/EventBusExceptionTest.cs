using System;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Events.Bus
{
    public class EventBusExceptionTest : EventBusTestBase
    {
        [Fact]
        public void ShouldThrowSingleExceptionIfOnlyOneOfHandlersFails()
        {
            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    throw new Exception("This exception is intentionally thrown!");
                });

            var appException = Assert.Throws<Exception>(() =>
            {
                EventBus.Trigger<MySimpleEventData>(null, new MySimpleEventData(1));
            });

            appException.Message.ShouldBe("This exception is intentionally thrown!");
        }

        [Fact]
        public void ShouldThrowAggregateExceptionIfMoreThanOneOfHandlersFail()
        {
            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    throw new Exception("This exception is intentionally thrown #1!");
                });

            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    throw new Exception("This exception is intentionally thrown #2!");
                });

            var aggrException = Assert.Throws<AggregateException>(() =>
            {
                EventBus.Trigger<MySimpleEventData>(null, new MySimpleEventData(1));
            });

            aggrException.InnerExceptions.Count.ShouldBe(2);
            aggrException.InnerExceptions[0].Message.ShouldBe("This exception is intentionally thrown #1!");
            aggrException.InnerExceptions[1].Message.ShouldBe("This exception is intentionally thrown #2!");
        }
    }
}