using System.Threading.Tasks;
using StudioX.TestBase;
using StudioX.Web.Configuration;
using Shouldly;
using Xunit;

namespace StudioX.Web.Common.Tests.Configuration
{
    public class StudioXUserConfigurationBuilderTests : StudioXIntegratedTestBase<StudioXWebCommonTestModule>
    {
        private readonly StudioXUserConfigurationBuilder studioXUserConfigurationBuilder;

        public StudioXUserConfigurationBuilderTests()
        {
            studioXUserConfigurationBuilder = Resolve<StudioXUserConfigurationBuilder>();
        }

        [Fact]
        public async Task StudioXUserConfigurationBuilderShouldBuildUserConfiguration()
        {
            var userConfiguration = await studioXUserConfigurationBuilder.GetAll();
            userConfiguration.ShouldNotBe(null);
            
            userConfiguration.MultiTenancy.ShouldNotBe(null);
            userConfiguration.Session.ShouldNotBe(null);
            userConfiguration.Localization.ShouldNotBe(null);
            userConfiguration.Features.ShouldNotBe(null);
            userConfiguration.Auth.ShouldNotBe(null);
            userConfiguration.Nav.ShouldNotBe(null);
            userConfiguration.Setting.ShouldNotBe(null);
            userConfiguration.Clock.ShouldNotBe(null);
            userConfiguration.Timing.ShouldNotBe(null);
            userConfiguration.Security.ShouldNotBe(null);
        }
    }
}
