using StudioX.Modules;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Startup
{
    public class StudioXBootstraperTester : TestBaseWithLocalIocManager
    {
        private readonly StudioXBootstrapper bootstrapper;

        public StudioXBootstraperTester()
        {
            bootstrapper = StudioXBootstrapper.Create<MyTestModule>(LocalIocManager);
        }

        [Fact]
        public void ShouldInitializeBootstrapper()
        {
            bootstrapper.Initialize();
        }

        [Fact]
        public void ShouldCallModuleEventsOnce()
        {
            bootstrapper.Initialize();
            bootstrapper.Dispose();

            var testModule = LocalIocManager.Resolve<MyTestModule>();
            var otherModule = LocalIocManager.Resolve<MyOtherModule>();
            var anotherModule = LocalIocManager.Resolve<MyAnotherModule>();

            testModule.PreInitializeCount.ShouldBe(1);
            testModule.InitializeCount.ShouldBe(1);
            testModule.PostInitializeCount.ShouldBe(1);
            testModule.ShutdownCount.ShouldBe(1);

            otherModule.PreInitializeCount.ShouldBe(1);
            otherModule.InitializeCount.ShouldBe(1);
            otherModule.PostInitializeCount.ShouldBe(1);
            otherModule.ShutdownCount.ShouldBe(1);
            otherModule.CallMeOnStartupCount.ShouldBe(1);

            anotherModule.PreInitializeCount.ShouldBe(1);
            anotherModule.InitializeCount.ShouldBe(1);
            anotherModule.PostInitializeCount.ShouldBe(1);
            anotherModule.ShutdownCount.ShouldBe(1);
        }

        public override void Dispose()
        {
            bootstrapper.Dispose();
            base.Dispose();
        }
    }

    [DependsOn(typeof(MyOtherModule))]
    [DependsOn(typeof(MyAnotherModule))]
    public class MyTestModule : MyEventCounterModuleBase
    {
        private readonly MyOtherModule otherModule;

        public MyTestModule(MyOtherModule otherModule)
        {
            this.otherModule = otherModule;
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
            otherModule.PreInitializeCount.ShouldBe(1);
            otherModule.CallMeOnStartup();
        }

        public override void Initialize()
        {
            base.Initialize();
            otherModule.InitializeCount.ShouldBe(1);
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            otherModule.PostInitializeCount.ShouldBe(1);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            otherModule.ShutdownCount.ShouldBe(0); //Depended module should be shutdown after this module
        }
    }

    public class MyOtherModule : MyEventCounterModuleBase
    {
        public int CallMeOnStartupCount { get; private set; }

        public void CallMeOnStartup()
        {
            CallMeOnStartupCount++;
        }
    }

    public class MyAnotherModule : MyEventCounterModuleBase
    {

    }

    public abstract class MyEventCounterModuleBase : StudioXModule
    {
        public int PreInitializeCount { get; private set; }

        public int InitializeCount { get; private set; }

        public int PostInitializeCount { get; private set; }

        public int ShutdownCount { get; private set; }

        public override void PreInitialize()
        {
            IocManager.ShouldNotBe(null);
            Configuration.ShouldNotBe(null);
            PreInitializeCount++;
        }

        public override void Initialize()
        {
            InitializeCount++;
        }

        public override void PostInitialize()
        {
            PostInitializeCount++;
        }

        public override void Shutdown()
        {
            ShutdownCount++;
        }
    }
}
