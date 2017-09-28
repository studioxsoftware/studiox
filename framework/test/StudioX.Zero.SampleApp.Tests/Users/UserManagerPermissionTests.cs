using System.Threading.Tasks;
using StudioX.IdentityFramework;
using StudioX.Threading;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserManagerPermissionTests : SampleAppTestBase
    {
        private readonly User testUser;
        private readonly Role role1;
        private readonly Role role2;

        public UserManagerPermissionTests()
        {
            role1 = CreateRole("Role1").Result;
            role2 = CreateRole("Role2").Result;
            testUser = CreateUser("TestUser").Result;
            AsyncHelper.RunSync(() => UserManager.AddToRolesAsync(testUser.Id, role1.Name, role2.Name)).CheckErrors();
        }

        [Fact]
        public async Task ShouldNotBeGrantedWithNoPermissionSetting()
        {
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task ShouldBeGrantedIfOneOfRolesIsGranted()
        {
            await GrantPermissionAsync(role1, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
        }

        [Fact]
        public async Task ShouldNotBeGrantedAfterGrantedRoleIsDeleted()
        {
            //Not granted initially
            (await IsGrantedAsync("Permission1")).ShouldBe(false);

            //Grant one role of the user
            await GrantPermissionAsync(role1, "Permission1");

            //Now, should be granted
            (await IsGrantedAsync("Permission1")).ShouldBe(true);

            //Delete the role
            await RoleManager.DeleteAsync(role1);

            //Now, should not be granted
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task ShouldBeGrantedIfGrantedForUser()
        {
            await GrantPermissionAsync(testUser, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
        }

        [Fact]
        public async Task ShouldNotBeGrantedIfProhibitedForUser()
        {
            //Permission3 is granted by default, but prohibiting for this user
            await ProhibitPermissionAsync(testUser, "Permission3");
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
        }

        [Fact]
        public async Task ShouldNotBeGrantedIfGrantedForRoleButProhibitedForUser()
        {
            await GrantPermissionAsync(role2, "Permission1");
            await ProhibitPermissionAsync(testUser, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task SetGrantedPermissionsAndResetTest()
        {
            await GrantPermissionAsync(role1, "Permission1");
            await GrantPermissionAsync(role2, "Permission2");

            //Set permissions

            await UserManager.SetGrantedPermissionsAsync(
                testUser,
                new[]
                {
                    PermissionManager.GetPermission("Permission1"),
                    PermissionManager.GetPermission("Permission4")
                });
            
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
            (await IsGrantedAsync("Permission2")).ShouldBe(false);
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
            (await IsGrantedAsync("Permission4")).ShouldBe(true);

            //Reset user-specific permissions

            await UserManager.ResetAllPermissionsAsync(testUser);

            (await IsGrantedAsync("Permission1")).ShouldBe(true); //Role1 has Permission1
            (await IsGrantedAsync("Permission2")).ShouldBe(true); //Role2 has Permission2
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
            (await IsGrantedAsync("Permission4")).ShouldBe(false);
        }

        [Fact]
        public async Task ProhibitAllPermissionsTest()
        {
            await GrantPermissionAsync(role1, "Permission1");
            await UserManager.ProhibitAllPermissionsAsync(testUser);
            foreach (var permission in PermissionManager.GetAllPermissions())
            {
                (await IsGrantedAsync(permission.Name)).ShouldBe(false);
            }
        }

        private async Task<bool> IsGrantedAsync(string permissionName)
        {
            return (await PermissionChecker.IsGrantedAsync(testUser.ToUserIdentifier(), permissionName));
        }
    }
}
