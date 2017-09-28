using StudioX.Domain.Uow;
using StudioX.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserManagerUpdateTests : SampleAppTestBase
    {
        private readonly UserManager userManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UserManagerUpdateTests()
        {
            userManager = Resolve<UserManager>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            StudioXSession.TenantId = 1; //Default tenant
        }

        [Fact]
        public void ShouldNotChangeAdminUserName()
        {
            using (var uow = unitOfWorkManager.Begin())
            {
                var adminUser = userManager.FindByName("admin");

                adminUser.UserName = "tuana";

                userManager.Update(adminUser).Succeeded.ShouldBeFalse();

                uow.Complete();
            }
        }
    }
}