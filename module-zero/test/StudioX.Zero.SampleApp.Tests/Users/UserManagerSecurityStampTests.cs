using System;
using System.Threading.Tasks;
using StudioX.Threading;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserManagerSecurityStampTests : SampleAppTestBase
    {
        private readonly UserManager userManager;
        private readonly User testUser;

        public UserManagerSecurityStampTests()
        {
            userManager = Resolve<UserManager>();

            userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromSeconds(0.5);

            testUser = AsyncHelper.RunSync(() => CreateUser("TestUser"));
        }

        [Fact]
        public void TestSecurityStamp()
        {
            userManager.SupportsUserSecurityStamp.ShouldBeTrue();
        }

        [Fact]
        public async Task TestSetGet()
        {
            var oldStamp = await userManager.GetSecurityStampAsync(testUser.Id);

            await userManager.UpdateSecurityStampAsync(testUser.Id);

            (await userManager.GetSecurityStampAsync(testUser.Id)).ShouldNotBe(oldStamp);
        }
    }
}