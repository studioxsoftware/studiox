using System;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Configuration;
using StudioX.Threading;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.Authorization;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserManagerLockoutTests : SampleAppTestBase
    {
        private readonly UserManager userManager;
        private readonly AppLogInManager logInManager;
        private readonly User testUser;

        public UserManagerLockoutTests()
        {
            userManager = Resolve<UserManager>();
            logInManager = Resolve<AppLogInManager>();

            testUser = AsyncHelper.RunSync(() => CreateUser("TestUser"));

            Resolve<ISettingManager>()
                .ChangeSettingForApplicationAsync(
                    StudioXZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    "1"
                );
        }

        [Fact]
        public void TestSupportsUserLockout()
        {
            userManager.SupportsUserLockout.ShouldBeTrue();
        }

        [Fact]
        public async Task TestLockoutFull()
        {
            userManager.InitializeLockoutSettings(testUser.TenantId);

            for (int i = 0; i < userManager.MaxFailedAccessAttemptsBeforeLockout; i++)
            {
                (await userManager.IsLockedOutAsync(testUser.Id)).ShouldBeFalse();
                await userManager.AccessFailedAsync(testUser.Id);
            }

            (await userManager.IsLockedOutAsync(testUser.Id)).ShouldBeTrue();

            await Task.Delay(TimeSpan.FromSeconds(1.5)); //Wait for unlock
            
            (await userManager.IsLockedOutAsync(testUser.Id)).ShouldBeFalse();
        }

        [Fact]
        public async Task TestLoginLockout()
        {
            (await logInManager.LoginAsync("TestUser", "123qwe")).Result.ShouldBe(StudioXLoginResultType.Success);

            for (int i = 0; i < userManager.MaxFailedAccessAttemptsBeforeLockout - 1; i++)
            {
                (await logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(StudioXLoginResultType.InvalidPassword);
            }

            (await logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(StudioXLoginResultType.LockedOut);
            (await userManager.IsLockedOutAsync(testUser.Id)).ShouldBeTrue();

            await Task.Delay(TimeSpan.FromSeconds(1.5)); //Wait for unlock

            (await userManager.GetAccessFailedCountAsync(testUser.Id)).ShouldBe(0);
            (await userManager.IsLockedOutAsync(testUser.Id)).ShouldBeFalse();
            (await logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(StudioXLoginResultType.InvalidPassword);

            (await logInManager.LoginAsync("TestUser", "123qwe")).Result.ShouldBe(StudioXLoginResultType.Success);
            (await userManager.GetAccessFailedCountAsync(testUser.Id)).ShouldBe(0);
        }
    }
}