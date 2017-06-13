using System.Data.Entity;
using System.Threading.Tasks;
using StudioX.IdentityFramework;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Roles
{
    public class RoleManagerDeleteTests : SampleAppTestBase
    {
        [Fact]
        public async Task UserShouldBeUnassignedFromRoleWhenTheRoleIsDeleted()
        {
            //Create user and roles
            var user1 = await CreateUser("User1");
            var role1 = await CreateRole("Role1");
            var role2= await CreateRole("Role2");

            //Add role1, role2 to the user
            (await UserManager.AddToRoleAsync(user1.Id, role1.Name)).CheckErrors();
            (await UserManager.AddToRoleAsync(user1.Id, role2.Name)).CheckErrors();

            //Check if user in role1
            (await UserManager.IsInRoleAsync(user1.Id, role1.Name)).ShouldBe(true);
            (await UserManager.GetRolesAsync(user1.Id)).ShouldContain("Role1");
            await UsingDbContext(
                async context =>
                {
                    (await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user1.Id && ur.RoleId == role1.Id)).ShouldNotBe(null);
                });

            //Delete role1
            (await RoleManager.DeleteAsync(role1)).CheckErrors();

            //Used should not be in role1
            (await UserManager.GetRolesAsync(user1.Id)).ShouldNotContain("Role1");
            await UsingDbContext(
                async context =>
                {
                    (await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user1.Id && ur.RoleId == role1.Id)).ShouldBe(null);
                });
        }
    }
}