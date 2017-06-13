using System.Collections.Generic;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Domain.Uow;
using StudioX.Runtime.Caching;
using StudioX.Zero.Configuration;
using StudioX.Boilerplate.Authorization.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
            IHttpContextAccessor contextAccessor, 
            IPermissionManager permissionManager, 
            ICacheManager cacheManager, 
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig)
            : base(
                  store,
                  roleValidators, 
                  keyNormalizer, 
                  errors, logger, 
                  contextAccessor, 
                  permissionManager,
                  cacheManager, 
                  unitOfWorkManager,
                  roleManagementConfig)
        {
        }
    }
}