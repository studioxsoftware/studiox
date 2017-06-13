using Shouldly;
using Xunit;

namespace StudioX.Tests
{
    public class DisposeActionTest
    {
        [Fact]
        public void ShouldCallActionWhenDisposed()
        {
            var actionIsCalled = false;
            
            using (new DisposeAction(() => actionIsCalled = true))
            {
                
            }

            actionIsCalled.ShouldBe(true);
        }
    }
}
