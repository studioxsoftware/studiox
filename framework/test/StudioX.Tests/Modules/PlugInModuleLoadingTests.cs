using System.Linq;
using StudioX.Modules;
using StudioX.PlugIns;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Modules
{
    public class PlugInModuleLoadingTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldLoadAllModules()
        {
            //Arrange
            var bootstrapper = StudioXBootstrapper.Create<MyStartupModule>(LocalIocManager);

            bootstrapper.PlugInSources.AddTypeList(typeof(MyPlugInModule));

            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<IStudioXModuleManager>().Modules;

            //Assert
            modules.Count.ShouldBe(6);

            modules.Any(m => m.Type == typeof(StudioXKernelModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule2)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInDependedModule)).ShouldBeTrue();

            modules.Any(m => m.Type == typeof(MyNotDependedModule)).ShouldBeFalse();
        }

        [DependsOn(typeof(MyModule1), typeof(MyModule2))]
        public class MyStartupModule: StudioXModule
        {

        }

        public class MyModule1 : StudioXModule
        {
            
        }

        public class MyModule2 : StudioXModule
        {

        }
        
        public class MyNotDependedModule : StudioXModule
        {

        }

        [DependsOn(typeof(MyPlugInDependedModule))]
        public class MyPlugInModule : StudioXModule
        {
            
        }

        public class MyPlugInDependedModule : StudioXModule
        {
            
        }
    }
}
