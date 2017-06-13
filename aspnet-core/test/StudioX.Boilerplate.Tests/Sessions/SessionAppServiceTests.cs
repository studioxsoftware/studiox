using System.Threading.Tasks;
using StudioX.Boilerplate.Sessions;
using Shouldly;
using Xunit;

namespace StudioX.Boilerplate.Tests.Sessions
{
    public class SessionAppServiceTests : BoilerplateTestBase
    {
        private readonly ISessionAppService sessionAppService;

        public SessionAppServiceTests()
        {
            sessionAppService = Resolve<ISessionAppService>();
        }

        [MultiTenantFact]
        public async Task ShouldGetCurrentUserWhenLoggedInAsHost()
        {
            //Arrange
            LoginAsHostAdmin();

            //Act
            var output = await sessionAppService.GetCurrentLoginInformations();

            //Assert
            var currentUser = await GetCurrentUserAsync();
            output.User.ShouldNotBe(null);
            output.User.FirstName.ShouldBe(currentUser.FirstName);
            output.User.LastName.ShouldBe(currentUser.LastName);

            output.Tenant.ShouldBe(null);
        }

        [Fact]
        public async Task ShouldGetCurrentUserAndTenantWhenLoggedInAsTenant()
        {
            //Act
            var output = await sessionAppService.GetCurrentLoginInformations();

            //Assert
            var currentUser = await GetCurrentUserAsync();
            var currentTenant = await GetCurrentTenantAsync();

            output.User.ShouldNotBe(null);
            output.User.FirstName.ShouldBe(currentUser.FirstName);

            output.Tenant.ShouldNotBe(null);
            output.Tenant.Name.ShouldBe(currentTenant.Name);
        }
    }
}
