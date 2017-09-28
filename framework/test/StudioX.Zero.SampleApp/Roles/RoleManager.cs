using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Domain.Uow;
using StudioX.Runtime.Caching;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Roles
{
    public class RoleManager : StudioXRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store, 
            IPermissionManager permissionManager, 
            IRoleManagementConfig roleManagementConfig, 
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
            store, 
            permissionManager, 
            roleManagementConfig, 
            cacheManager,
            unitOfWorkManager)
        {
        }
    }
}