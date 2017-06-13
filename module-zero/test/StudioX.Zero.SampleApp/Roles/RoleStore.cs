using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Domain.Repositories;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Roles
{
    public class RoleStore : StudioXRoleStore<Role, User>
    {
        public RoleStore(
            IRepository<Role> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                roleRepository,
                userRoleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}