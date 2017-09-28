using System.Linq;
using StudioX.Timing;
using StudioX.Zero.SampleApp.Users;
using StudioX.Zero.SampleApp.Users.Dto;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserAccountSynchronizerTests : SampleAppTestBase
    {
        private readonly IUserAppService userAppService;

        public UserAccountSynchronizerTests()
        {
            userAppService = Resolve<IUserAppService>();
        }

        [Fact]
        public void ShouldCreateUserAccountWhenUserCreated()
        {
            var user = CreateAndGetUser();

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.FirstOrDefault(u => u.UserName == "yunus.emre");
                    userAccount.ShouldNotBe(null);
                    userAccount.UserId.ShouldBe(user.Id);
                    userAccount.TenantId.ShouldBe(user.TenantId);
                    userAccount.UserName.ShouldBe(user.UserName);
                    userAccount.EmailAddress.ShouldBe(user.EmailAddress);
                    userAccount.LastLoginTime.ShouldBe(user.LastLoginTime);
                });
        }

        [Fact]
        public void ShouldUpdateUserAccountWhenUserUpdated()
        {
            var user = CreateAndGetUser();
            var now = Clock.Now;
            userAppService.UpdateUser(new UpdateUserInput
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = "y.emre",
                EmailAddress = "y.emre@studioxsoftware.com",
                LastLoginTime = now
            });

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.FirstOrDefault(u => u.UserName == "y.emre");
                    userAccount.ShouldNotBe(null);
                    userAccount.UserId.ShouldBe(user.Id);
                    userAccount.TenantId.ShouldBe(user.TenantId);
                    userAccount.UserName.ShouldBe("y.emre");
                    userAccount.EmailAddress.ShouldBe("y.emre@studioxsoftware.com");
                    userAccount.LastLoginTime.ShouldBe(now);
                });
        }

        [Fact]
        public void ShouldDeleteUserAccountWhenUserDeleted()
        {
            var user = CreateAndGetUser();

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "yunus.emre");
                    userAccount.IsDeleted.ShouldBe(false);
                });

            userAppService.DeleteUser(user.Id);

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "yunus.emre");
                    userAccount.IsDeleted.ShouldBe(true);
                });
        }

        private User CreateAndGetUser()
        {
            userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "emre@studioxsoftware.com",
                    FirstName = "Yunus",
                    LastName = "Emre",
                    UserName = "yunus.emre"
                });

            return UsingDbContext(
                context =>
                {
                    return context.Users.First(u => u.UserName == "yunus.emre");
                });
        }
    }
}
