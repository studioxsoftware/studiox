using StudioX.Configuration.Startup;
using StudioX.Runtime.Session;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Session
{
    public class SessionTests : SampleApplicationTestBase
    {
        private readonly IStudioXSession session;

        public SessionTests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            session = Resolve<IStudioXSession>();
        }

        [Fact]
        public void SessionOverrideTest()
        {
            session.UserId.ShouldBeNull();
            session.TenantId.ShouldBeNull();

            using (session.Use(42, 571))
            {
                session.TenantId.ShouldBe(42);
                session.UserId.ShouldBe(571);

                using (session.Use(null, 3))
                {
                    session.TenantId.ShouldBeNull();
                    session.UserId.ShouldBe(3);
                }

                session.TenantId.ShouldBe(42);
                session.UserId.ShouldBe(571);
            }

            session.UserId.ShouldBeNull();
            session.TenantId.ShouldBeNull();
        }
    }
}
