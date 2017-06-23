using System.Threading.Tasks;
using StudioX.Boilerplate.Users;
using StudioX.Boilerplate.Users.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace StudioX.Boilerplate.Tests.Users
{
    public class UserAppServiceTests : BoilerplateTestBase
    {
        private readonly IUserAppService userAppService;

        public UserAppServiceTests()
        {
            userAppService = Resolve<IUserAppService>();
        }

        [Fact]
        public async Task GetUsersTest()
        {
            //Act
            var output = await userAppService.GetAll();

            //Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateUserTest()
        {
            //Act
            await userAppService.Create(
                new CreateUserInput
                {
                    EmailAddress = "long@studiox.com",
                    IsActive = true,
                    FirstName = "Long",
                    LastName = "Huynh",
                    Password = "123qwe",
                    UserName = "long.huynh"
                });

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "long.huynh");
                johnNashUser.ShouldNotBeNull();
            });
        }
    }
}
