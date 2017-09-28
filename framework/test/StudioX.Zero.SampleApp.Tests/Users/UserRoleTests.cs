using System.Threading.Tasks;
using Shouldly;
using StudioX.Authorization.Users;
using StudioX.Domain.Uow;
using StudioX.IdentityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserRoleTests : SampleAppTestBase
    {
        public UserRoleTests()
        {
            UsingDbContext(
                context =>
                {
                    var tenant1 = context.Tenants.Add(new Tenant("tenant1", "Tenant one"));
                    context.SaveChanges();

                    StudioXSession.TenantId = tenant1.Id;

                    var user1 = context.Users.Add(new User
                    {
                        TenantId = StudioXSession.TenantId,
                        UserName = "user1",
                        FirstName = "User",
                        LastName = "One",
                        EmailAddress = "user-one@studioxsoftware.com",
                        IsEmailConfirmed = true,
                        Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw=="
                        //123qwe
                    });
                    context.SaveChanges();

                    var role1 = context.Roles.Add(new Role(StudioXSession.TenantId, "role1", "Role 1"));
                    var role2 = context.Roles.Add(new Role(StudioXSession.TenantId, "role2", "Role 1"));
                    context.SaveChanges();

                    context.UserRoles.Add(new UserRole(StudioXSession.TenantId, user1.Id, role1.Id));
                });
        }

        [Fact]
        public async Task ShouldChangeRoles()
        {
            var unitOfWorkManager = LocalIocManager.Resolve<IUnitOfWorkManager>();
            using (var uow = unitOfWorkManager.Begin())
            {
                var user = await UserManager.FindByNameAsync("user1");

                //Check initial role assignments
                var roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldContain("role1");
                roles.ShouldNotContain("role2");

                //Delete all role assignments
                (await UserManager.RemoveFromRolesAsync(user.Id, "role1")).CheckErrors();
                await unitOfWorkManager.Current.SaveChangesAsync();

                //Check role assignments again
                roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldNotContain("role1");
                roles.ShouldNotContain("role2");

                //Add to roles
                (await UserManager.AddToRolesAsync(user.Id, "role1", "role2")).CheckErrors();
                await unitOfWorkManager.Current.SaveChangesAsync();

                //Check role assignments again
                roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldContain("role1");
                roles.ShouldContain("role2");

                await uow.CompleteAsync();
            }
        }
    }
}