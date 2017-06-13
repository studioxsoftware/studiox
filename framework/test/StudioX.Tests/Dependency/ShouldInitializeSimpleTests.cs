using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class ShouldInitializeSimpleTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldCallInitialize()
        {
            LocalIocManager.Register<MyService>(DependencyLifeStyle.Transient);
            var myService = LocalIocManager.Resolve<MyService>();
            myService.InitializeCount.ShouldBe(1);
        }

        public class MyService : IShouldInitialize
        {
            public int InitializeCount { get; private set; }

            public void Initialize()
            {
                InitializeCount++;
            }
        }
    }
}
