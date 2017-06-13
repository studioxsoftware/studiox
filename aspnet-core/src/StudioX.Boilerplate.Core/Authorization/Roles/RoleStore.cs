using StudioX.Authorization.Roles;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Authorization.Roles
{
    public class RoleStore : StudioXRoleStore<Role, User>
    {
        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}