using System.Linq;
using System.Threading.Tasks;
using StudioX.IdentityFramework;
using StudioX.Organizations;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserOrganizationUnitTests : SampleAppTestBase
    {
        private readonly UserManager userManager;
        private readonly Tenant defaultTenant;
        private readonly User defaultTenantAdmin;

        public UserOrganizationUnitTests()
        {
            defaultTenant = GetDefaultTenant();
            defaultTenantAdmin = GetDefaultTenantAdmin();

            StudioXSession.TenantId = defaultTenant.Id;
            StudioXSession.UserId = defaultTenantAdmin.Id;

            userManager = Resolve<UserManager>();
        }

        [Fact]
        public async Task TestIsInOrganizationUnitAsync()
        {
            //Act & Assert
            (await userManager.IsInOrganizationUnitAsync(defaultTenantAdmin, GetOU("OU11"))).ShouldBe(true);
            (await userManager.IsInOrganizationUnitAsync(defaultTenantAdmin, GetOU("OU2"))).ShouldBe(false);
        }

        [Fact]
        public async Task TestAddToOrganizationUnitAsync()
        {
            //Arrange
            var ou2 = GetOU("OU2");

            //Act
            await userManager.AddToOrganizationUnitAsync(defaultTenantAdmin, ou2);

            //Assert
            (await userManager.IsInOrganizationUnitAsync(defaultTenantAdmin, ou2)).ShouldBe(true);
            UsingDbContext(context => context.UserOrganizationUnits.FirstOrDefault(ou => ou.UserId == defaultTenantAdmin.Id && ou.OrganizationUnitId == ou2.Id).ShouldNotBeNull());
        }

        [Fact]
        public async Task TestRemoveFromOrganizationUnitAsync()
        {
            //Arrange
            var ou11 = GetOU("OU11");

            //Act
            await userManager.RemoveFromOrganizationUnitAsync(defaultTenantAdmin, ou11);

            //Assert
            (await userManager.IsInOrganizationUnitAsync(defaultTenantAdmin, ou11)).ShouldBe(false);
            UsingDbContext(context => context.UserOrganizationUnits.FirstOrDefault(ou => ou.UserId == defaultTenantAdmin.Id && ou.OrganizationUnitId == ou11.Id).ShouldBeNull());
        }

        [Fact]
        public async Task ShouldRemoveUserFromOrganizationWhenUserIsDeleted()
        {

            //Arrange
            var user = CreateAndGetTestUser();
            var ou11 = GetOU("OU11");

            await userManager.AddToOrganizationUnitAsync(user, ou11);
            (await userManager.IsInOrganizationUnitAsync(user, ou11)).ShouldBe(true);

            //Act
            (await userManager.DeleteAsync(user)).CheckErrors();

            //Assert
            (await userManager.IsInOrganizationUnitAsync(user, ou11)).ShouldBe(false);
        }

        [Theory]
        [InlineData(new object[] { new string[0] })]
        [InlineData(new object[] { new[] { "OU12", "OU21" } })]
        [InlineData(new object[] { new[] { "OU11", "OU12", "OU2" } })]
        public async Task TestSetOrganizationUnitsAsync(string[] organizationUnitNames)
        {
            //Arrange
            var organizationUnitIds = organizationUnitNames.Select(oun => GetOU(oun).Id).ToArray();

            //Act
            await userManager.SetOrganizationUnitsAsync(defaultTenantAdmin, organizationUnitIds);

            //Assert
            UsingDbContext(context =>
            {
                context.UserOrganizationUnits
                    .Count(uou => uou.UserId == defaultTenantAdmin.Id && organizationUnitIds.Contains(uou.OrganizationUnitId))
                    .ShouldBe(organizationUnitIds.Length);
            });
        }

        [Fact]
        public async Task TestGetUsersInOrganizationUnit()
        {
            //Act & Assert
            (await userManager.GetUsersInOrganizationUnit(GetOU("OU11"))).Count.ShouldBe(1);
            (await userManager.GetUsersInOrganizationUnit(GetOU("OU1"))).Count.ShouldBe(0);
            (await userManager.GetUsersInOrganizationUnit(GetOU("OU1"), true)).Count.ShouldBe(1);
        }

        private OrganizationUnit GetOU(string diplayName)
        {
            var organizationUnit = UsingDbContext(context => context.OrganizationUnits.FirstOrDefault(ou => ou.DisplayName == diplayName));
            organizationUnit.ShouldNotBeNull();

            return organizationUnit;
        }

        private User CreateAndGetTestUser()
        {
            userManager.Create(
                new User
                {
                    EmailAddress = "emre@studioxsoftware.com",
                    FirstName = "Yunus",
                    LastName = "Emre",
                    UserName = "yunus.emre",
                    IsEmailConfirmed = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                });

            return UsingDbContext(
                context =>
                {
                    return context.Users.Single(u => u.UserName == "yunus.emre");
                });
        }
    }
}
