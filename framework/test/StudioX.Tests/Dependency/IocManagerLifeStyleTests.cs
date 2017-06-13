using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class IocManagerLifeStyleTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldCallDisposeOfTransientDependencyWhenObjectIsReleased()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleTransient()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.IocContainer.Release(obj);

            obj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldCallDisposeOfTransientDependencyWhenIocManagerIsDisposed()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleTransient()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.Dispose();

            obj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldCallDisposeOfSingletonDependencyWhenIocManagerIsDisposed()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleSingleton()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.Dispose();

            obj.DisposeCount.ShouldBe(1);
        }
    }
}
