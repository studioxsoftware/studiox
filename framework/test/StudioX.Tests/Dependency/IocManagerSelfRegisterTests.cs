using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class IocManagerSelfRegisterTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldSelfRegisterWithAllInterfaces()
        {
            var registrar = LocalIocManager.Resolve<IIocRegistrar>();
            var resolver = LocalIocManager.Resolve<IIocResolver>();
            var managerByInterface = LocalIocManager.Resolve<IIocManager>();
            var managerByClass = LocalIocManager.Resolve<IocManager>();

            managerByClass.ShouldBeSameAs(registrar);
            managerByClass.ShouldBeSameAs(resolver);
            managerByClass.ShouldBeSameAs(managerByInterface);
        }
    }
}