using StudioX.Domain.Repositories;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserRepositoryTests : SampleAppTestBase
    {
        [Fact]
        public void ShouldInsertAndRetrieveUser()
        {
            var userRepository = LocalIocManager.Resolve<IRepository<User, long>>();

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@studioxsoftware.com").ShouldBe(null);

            userRepository.Insert(new User
            {
                TenantId = null,
                UserName = "admin",
                FirstName = "System",
                LastName = "Administrator",
                EmailAddress = "admin@studioxsoftware.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@studioxsoftware.com").ShouldNotBe(null);
        }
    }
}
