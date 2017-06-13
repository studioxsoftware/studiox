using System.Linq;

using StudioX.Modules;
using StudioX.PlugIns;

using Shouldly;

using Xunit;

namespace StudioX.Tests.Modules
{
    public class StartupModuleToBeLastTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void StartupModuleShouldBeLastModule()
        {
            //Arrange
            var bootstrapper = StudioXBootstrapper.Create<MyStartupModule>(LocalIocManager);
            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<IStudioXModuleManager>().Modules;

            //Assert
            modules.Count.ShouldBe(4);

            modules.Any(m => m.Type == typeof(StudioXKernelModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule2)).ShouldBeTrue();

            var startupModule = modules.Last();

            startupModule.Type.ShouldBe(typeof(MyStartupModule));
        }

        [Fact]
        public void PluginModuleShouldNotBeLast()
        {
            var bootstrapper = StudioXBootstrapper.Create<MyStartupModule>(LocalIocManager);

            bootstrapper.PlugInSources.AddTypeList(typeof(MyPlugInModule));

            bootstrapper.Initialize();

            var modules = bootstrapper.IocManager.Resolve<IStudioXModuleManager>().Modules;

            //Assert
            modules.Count.ShouldBe(6);

            modules.Any(m => m.Type == typeof(StudioXKernelModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule2)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInDependedModule)).ShouldBeTrue();

            modules.Last().Type.ShouldBe(typeof(MyStartupModule));
        }

        [DependsOn(typeof(MyModule1), typeof(MyModule2))]
        public class MyStartupModule : StudioXModule {}

        public class MyModule1 : StudioXModule {}

        public class MyModule2 : StudioXModule {}

        [DependsOn(typeof(MyPlugInDependedModule))]
        public class MyPlugInModule : StudioXModule {}

        public class MyPlugInDependedModule : StudioXModule {}
    }
}
