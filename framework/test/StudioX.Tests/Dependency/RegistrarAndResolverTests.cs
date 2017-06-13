using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class RegistrarAndResolverTests : TestBaseWithLocalIocManager
    {
        private readonly IIocRegistrar registrar;
        private readonly IIocResolver resolver;

        public RegistrarAndResolverTests()
        {
            registrar = LocalIocManager.Resolve<IIocRegistrar>();
            resolver = LocalIocManager.Resolve<IIocResolver>();
        }

        [Fact]
        public void ShouldResolveSelfRegisteredTypes()
        {
            registrar.Register<MyClass>();
            resolver.Resolve<MyClass>();
        }

        [Fact]
        public void ShouldResolveRegisteredByInterfaceTypes()
        {
            registrar.Register<IMyInterface, MyClass>();
            resolver.Resolve<IMyInterface>();

            try
            {
                resolver.Resolve<MyClass>();
                Assert.False(true, "Should not resolve by class that is registered by interface");
            }
            catch { }
        }

        [Fact]
        public void ShouldGetDifferentObjectsForTransients()
        {
            registrar.Register<MyClass>(DependencyLifeStyle.Transient);
            
            var obj1 = resolver.Resolve<MyClass>();
            var obj2 = resolver.Resolve<MyClass>();

            obj1.ShouldNotBeSameAs(obj2);
        }
        [Fact]
        public void ShouldGetSameObjectForSingleton()
        {
            registrar.Register<MyClass>(DependencyLifeStyle.Singleton);

            var obj1 = resolver.Resolve<MyClass>();
            var obj2 = resolver.Resolve<MyClass>();

            obj1.ShouldBeSameAs(obj2);
        }


        public class MyClass : IMyInterface
        {
            
        }

        public interface IMyInterface
        {

        }
    }
}