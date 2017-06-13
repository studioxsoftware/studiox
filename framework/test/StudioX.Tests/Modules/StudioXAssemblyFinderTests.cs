using System.Linq;
using System.Reflection;
using StudioX.Modules;
using StudioX.Reflection;
using StudioX.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Modules
{
    public class StudioXAssemblyFinderTests: TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldGetModuleAndAdditionalAssemblies()
        {
            //Arrange
            var bootstrapper = StudioXBootstrapper.Create<MyStartupModule>(LocalIocManager);
            bootstrapper.Initialize();

            //Act
            var assemblies = bootstrapper.IocManager.Resolve<StudioXAssemblyFinder>().GetAllAssemblies();

            //Assert
            assemblies.Count.ShouldBe(3);

            assemblies.Any(a => a == typeof(MyStartupModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(StudioXKernelModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(FactAttribute).GetAssembly()).ShouldBeTrue();
        }

        public class MyStartupModule : StudioXModule
        {
            public override Assembly[] GetAdditionalAssemblies()
            {
                return new[] {typeof(FactAttribute).GetAssembly()};
            }
        }
    }
}