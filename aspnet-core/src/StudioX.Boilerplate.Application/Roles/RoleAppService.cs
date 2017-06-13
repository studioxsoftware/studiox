using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Roles
{
    public class RoleAppService : BoilerplateAppServiceBase,IRoleAppService
    {
        private readonly RoleManager roleManager;
        private readonly IPermissionManager permissionManager;

        public RoleAppService(RoleManager roleManager, IPermissionManager permissionManager)
        {
            this.roleManager = roleManager;
            this.permissionManager = permissionManager;
        }

        public async Task UpdateRolePermissions(UpdateRolePermissionsInput input)
        {
            var role = await roleManager.GetRoleByIdAsync(input.RoleId);
            var grantedPermissions = permissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }
    }
}