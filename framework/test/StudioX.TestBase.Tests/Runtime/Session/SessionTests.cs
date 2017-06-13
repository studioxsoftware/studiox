using StudioX.Configuration.Startup;
using StudioX.Runtime.Session;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.Tests.Runtime.Session
{
    public class SessionTests : StudioXIntegratedTestBase<StudioXKernelModule>
    {
        [Fact]
        public void ShouldBeDefaultOnStartup()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            StudioXSession.UserId.ShouldBe(null);
            StudioXSession.TenantId.ShouldBe(1);

            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            StudioXSession.UserId.ShouldBe(null);
            StudioXSession.TenantId.ShouldBe(null);
        }

        [Fact]
        public void CanChangeSessionVariables()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            StudioXSession.UserId = 1;
            StudioXSession.TenantId = 42;

            var resolvedStudioXSession = LocalIocManager.Resolve<IStudioXSession>();

            resolvedStudioXSession.UserId.ShouldBe(1);
            resolvedStudioXSession.TenantId.ShouldBe(42);

            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            StudioXSession.UserId.ShouldBe(1);
            StudioXSession.TenantId.ShouldBe(1);
        }
    }
}
