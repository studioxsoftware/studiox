using System;
using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class ScopedIocResolverInjectTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldAutomaticallyReleaseResolvedDependenciesWhenInjectedClassReleased()
        {
            //Arrange
            LocalIocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyMainClass>(DependencyLifeStyle.Transient);

            //Act
            var mainClass = LocalIocManager.Resolve<MyMainClass>();
            var dependency = mainClass.CreateDependency();
            dependency.IsDisposed.ShouldBeFalse();
            LocalIocManager.Release(mainClass);

            //Assert
            dependency.IsDisposed.ShouldBeTrue();
        }

        public class MyDependency : IDisposable
        {
            public bool IsDisposed { get; set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public class MyMainClass
        {
            private readonly IScopedIocResolver resolver;

            public MyMainClass(IScopedIocResolver resolver)
            {
                this.resolver = resolver;
            }

            public MyDependency CreateDependency()
            {
                return resolver.Resolve<MyDependency>();
            }
        }
    }
}
