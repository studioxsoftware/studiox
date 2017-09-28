using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Domain.Uow;
using StudioX.Runtime.Caching;
using StudioX.Zero.Configuration;

namespace StudioX.Boilerplate.Authorization.Roles
{
    public class RoleManager : StudioXRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<StudioXRoleManager<Role, User>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors, logger,
                permissionManager,
                cacheManager,
                unitOfWorkManager,
                roleManagementConfig)
        {
        }
    }
}