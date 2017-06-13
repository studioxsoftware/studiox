using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Runtime.Session;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.Authorization;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserLoginTests : SampleAppTestBase
    {
        private readonly AppLogInManager logInManager;

        public UserLoginTests()
        {
            UsingDbContext(UserLoginHelper.CreateTestUsers);
            logInManager = LocalIocManager.Resolve<AppLogInManager>();
        }

        [Fact]
        public async Task ShouldLoginWithCorrectValuesWithoutMultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;
            StudioXSession.TenantId = 1;

            var loginResult = await logInManager.LoginAsync("user1", "123qwe");
            loginResult.Result.ShouldBe(StudioXLoginResultType.Success);
            loginResult.User.FirstName.ShouldBe("User");
            loginResult.Identity.ShouldNotBe(null);

            UsingDbContext(context =>
            {
                context.UserLoginAttempts.Count().ShouldBe(1);
                context.UserLoginAttempts.FirstOrDefault(a => 
                    a.TenantId == StudioXSession.TenantId &&
                    a.UserId == loginResult.User.Id &&
                    a.UserNameOrEmailAddress == "user1" &&
                    a.Result == StudioXLoginResultType.Success
                    ).ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ShouldNotLoginWithInvalidUserNameWithoutMultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            var loginResult = await logInManager.LoginAsync("wrongUserName", "asdfgh");
            loginResult.Result.ShouldBe(StudioXLoginResultType.InvalidUserNameOrEmailAddress);
            loginResult.User.ShouldBe(null);
            loginResult.Identity.ShouldBe(null);
            
            UsingDbContext(context =>
            {
                context.UserLoginAttempts.Count().ShouldBe(1);
                context.UserLoginAttempts.FirstOrDefault(a =>
                    a.UserNameOrEmailAddress == "wrongUserName" &&
                    a.Result == StudioXLoginResultType.InvalidUserNameOrEmailAddress
                    ).ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ShouldLoginWithCorrectValuesWithMultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            StudioXSession.TenantId = 1;

            var loginResult = await logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName);
            loginResult.Result.ShouldBe(StudioXLoginResultType.Success);
            loginResult.User.FirstName.ShouldBe("User");
            loginResult.Identity.ShouldNotBe(null);
        }

        [Fact]
        public async Task ShouldNotLoginIfEmailConfirmationIsEnabledAndUserHasNotConfirmed()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            //Set session
            StudioXSession.TenantId = 1;
            StudioXSession.UserId = 1;

            //Email confirmation is disabled as default
            (await logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(StudioXLoginResultType.Success);

            //Change configuration
            await Resolve<ISettingManager>().ChangeSettingForTenantAsync(StudioXSession.GetTenantId(), StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, "true");

            //Email confirmation is enabled now
            (await logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(StudioXLoginResultType.UserEmailIsNotConfirmed);
        }

        [Fact]
        public async Task ShouldLoginTenancyOwnerWithCorrectValues()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            var loginResult = await logInManager.LoginAsync("userOwner", "123qwe");
            loginResult.Result.ShouldBe(StudioXLoginResultType.Success);
            loginResult.User.FirstName.ShouldBe("Owner");
            loginResult.Identity.ShouldNotBe(null);
        }
    }
}
