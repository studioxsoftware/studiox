using System.Threading.Tasks;
using StudioX.Configuration.Startup;
using StudioX.MultiTenancy;
using StudioX.Runtime.Remoting;
using StudioX.Runtime.Session;
using StudioX.TestBase.Runtime.Session;
using StudioX.Tests.Application.Navigation;
using StudioX.Web.Navigation;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.Web.Tests.Navigation
{
    public class NavigationScriptTests
    {
        [Fact]
        public async Task ShouldGetScript()
        {
            var testCase = new NavigationTestCase();
            var scriptManager = new NavigationScriptManager(testCase.UserNavigationManager)
            {
                StudioXSession = CreateTestStudioXSession()
            };

            var script = await scriptManager.GetScriptAsync();
            script.ShouldNotBeNullOrEmpty();
        }

        private static TestStudioXSession CreateTestStudioXSession()
        {
            return new TestStudioXSession(
                new MultiTenancyConfig { IsEnabled = true },
                new DataContextAmbientScopeProvider<SessionOverride>(
#if NET46
                new CallContextAmbientDataContext()
#else
                new AsyncLocalAmbientDataContext()
#endif
                    ),
                Substitute.For<ITenantResolver>()
            );
        }
    }
}
