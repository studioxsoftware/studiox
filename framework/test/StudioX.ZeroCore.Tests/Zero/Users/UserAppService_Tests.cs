using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.ZeroCore.SampleApp.Application.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.Users
{
    public class UserAppServiceTests: StudioXZeroTestBase
    {
        private readonly IUserAppService userAppService;

        public UserAppServiceTests()
        {
            userAppService = Resolve<IUserAppService>();
        }

        [Fact]
        public async Task ShouldGetAllUsers()
        {
            var users = await userAppService.GetAll(new PagedAndSortedResultRequestDto());
            users.TotalCount.ShouldBeGreaterThan(0);
            users.Items.Count.ShouldBeGreaterThan(0);
        }
    }
}
