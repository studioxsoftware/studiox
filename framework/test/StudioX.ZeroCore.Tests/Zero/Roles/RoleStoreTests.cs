using System.Threading.Tasks;
using StudioX.Domain.Uow;
using StudioX.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace StudioX.Zero.Roles
{
    public class RoleStoreTests : StudioXZeroTestBase
    {
        private readonly RoleStore roleStore;

        public RoleStoreTests()
        {
            roleStore = Resolve<RoleStore>();
        }

        [Fact]
        public async Task Should_Get_Role_Claims()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var role = await roleStore.FindByNameAsync("ADMIN");
                role.ShouldNotBeNull();

                var claims = await roleStore.GetClaimsAsync(role);

                claims.ShouldNotBeNull();

                await uow.CompleteAsync();
            }
        }
    }
}
