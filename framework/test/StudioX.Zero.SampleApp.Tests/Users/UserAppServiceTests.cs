using System.Linq;
using System.Threading.Tasks;
using StudioX.Auditing;
using StudioX.Zero.SampleApp.Users;
using StudioX.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserAppServiceTests : SampleAppTestBase
    {
        private readonly IUserAppService userAppService;
        private readonly UserManager userManager;

        public UserAppServiceTests()
        {
            userAppService = Resolve<IUserAppService>();
            userManager = Resolve<UserManager>();
            Resolve<IAuditingConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        [Fact]
        public void ShouldInsertAndWriteAuditLogs()
        {
            userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "emre@studioxsoftware.com",
                    FirstName = "Yunus",
                    LastName = "Emre",
                    UserName = "yunus.emre"
                });

            UsingDbContext(
                context =>
                {
                    context.Users.FirstOrDefault(u => u.UserName == "yunus.emre").ShouldNotBe(null);

                    var auditLog = context.AuditLogs.FirstOrDefault();
                    auditLog.ShouldNotBe(null);
                    auditLog.TenantId.ShouldBe(StudioXSession.TenantId);
                    auditLog.UserId.ShouldBe(StudioXSession.UserId);
                    auditLog.ServiceName.ShouldBe(typeof(UserAppService).FullName);
                    auditLog.MethodName.ShouldBe("CreateUser");
                    auditLog.Exception.ShouldBe(null);
                });
        }

        [Fact]
        public async Task ShoudlResetPassword()
        {
            StudioXSession.TenantId = 1; //Default tenant   
            var managerUser = await userManager.FindByNameAsync("manager");
            managerUser.PasswordResetCode = "fc9640bb73ec40a2b42b479610741a5a";
            userManager.Update(managerUser);

            StudioXSession.TenantId = null; //Default tenant  

            await userAppService.ResetPassword(new ResetPasswordInput
            {
                TenantId = 1,
                UserId = managerUser.Id,
                Password = "123qwe",
                ResetCode = "fc9640bb73ec40a2b42b479610741a5a"
            });

            var updatedUser = UsingDbContext(
                context =>
                {
                    return context.Users.FirstOrDefault(u => u.UserName == "manager");
                });

            updatedUser.UserName.ShouldBe("manager");
            updatedUser.PasswordResetCode.ShouldBe(null);
        }
    }
}
