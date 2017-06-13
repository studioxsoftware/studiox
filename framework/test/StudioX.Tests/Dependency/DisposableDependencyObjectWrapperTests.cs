using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class DisposableDependencyObjectWrapperTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ResolveAsDisposableShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj;

            using (var wrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableObject>())
            {
                wrapper.Object.ShouldNotBe(null);
                simpleObj = wrapper.Object;
            }

            simpleObj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void ResolveAsDisposableWithConstructorArgsShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            using (var wrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableObject>(new { myData = 42 }))
            {
                wrapper.Object.MyData.ShouldBe(42);
            }
        }

        [Fact]
        public void UsingTest()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);
            LocalIocManager.Using<SimpleDisposableObject>(obj => obj.MyData.ShouldBe(0));
        }
    }
}
