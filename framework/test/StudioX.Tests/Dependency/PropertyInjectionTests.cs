using StudioX.Application.Services;
using StudioX.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class PropertyInjectionTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldInjectSessionForApplicationService()
        {
            var session = Substitute.For<IStudioXSession>();
            session.TenantId.Returns(1);
            session.UserId.Returns(42);

            LocalIocManager.Register<MyApplicationService>();
            LocalIocManager.IocContainer.Register(
                Component.For<IStudioXSession>().Instance(session)
                );

            var myAppService = LocalIocManager.Resolve<MyApplicationService>();
            myAppService.TestSession();
        }

        private class MyApplicationService : ApplicationService
        {
            public void TestSession()
            {
                StudioXSession.ShouldNotBe(null);
                StudioXSession.TenantId.ShouldBe(1);
                StudioXSession.UserId.ShouldBe(42);
            }
        }
    }
}
