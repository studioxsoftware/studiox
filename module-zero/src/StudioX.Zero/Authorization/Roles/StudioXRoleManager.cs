using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization.Users;
using StudioX.Domain.Services;
using StudioX.Domain.Uow;
using StudioX.IdentityFramework;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Session;
using StudioX.Zero;
using StudioX.Zero.Configuration;
using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Roles
{
    /// <summary>
    /// Extends <see cref="RoleManager{TRole,TKey}"/> of ASP.NET Identity Framework.
    /// Applications should derive this class with appropriate generic arguments.
    /// </summary>
    public abstract class StudioXRoleManager<TRole, TUser>
        : RoleManager<TRole, int>,
        IDomainService
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        public ILocalizationManager LocalizationManager { get; set; }

        public IStudioXSession StudioXSession { get; set; }

        public IRoleManagementConfig RoleManagementConfig { get; private set; }

        private IRolePermissionStore<TRole> RolePermissionStore
        {
            get
            {
                if (!(Store is IRolePermissionStore<TRole>))
                {
                    throw new StudioXException("Store is not IRolePermissionStore");
                }

                return Store as IRolePermissionStore<TRole>;
            }
        }

        protected StudioXRoleStore<TRole, TUser> StudioXStore { get; private set; }

        private readonly IPermissionManager permissionManager;
        private readonly ICacheManager cacheManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected StudioXRoleManager(
            StudioXRoleStore<TRole, TUser> store,
            IPermissionManager permissionManager,
            IRoleManagementConfig roleManagementConfig,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(store)
        {
            this.permissionManager = permissionManager;
            this.cacheManager = cacheManager;
            this.unitOfWorkManager = unitOfWorkManager;

            RoleManagementConfig = roleManagementConfig;
            StudioXStore = store;
            StudioXSession = NullStudioXSession.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleName">The role's name to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(string roleName, string permissionName)
        {
            return await IsGrantedAsync((await GetRoleByNameAsync(roleName)).Id, permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        /// Checks if a role has a permission.
        /// </summary>
        /// <param name="roleId">The role's id to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, string permissionName)
        {
            return await IsGrantedAsync(roleId, permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public Task<bool> IsGrantedAsync(TRole role, Permission permission)
        {
            return IsGrantedAsync(role.Id, permission);
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleId">role id</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        /// <summary>
        /// Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(int roleId)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId));
        }

        /// <summary>
        /// Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(string roleName)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByNameAsync(roleName));
        }

        /// <summary>
        /// Gets granted permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TRole role)
        {
            var permissionList = new List<Permission>();

            foreach (var permission in permissionManager.GetAllPermissions())
            {
                if (await IsGrantedAsync(role.Id, permission))
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(int roleId, IEnumerable<Permission> permissions)
        {
            await SetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId), permissions);
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(TRole role, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(role);
            var newPermissions = permissions.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await ProhibitPermissionAsync(role, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await GrantPermissionAsync(role, permission);
            }
        }

        /// <summary>
        /// Grants a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task GrantPermissionAsync(TRole role, Permission permission)
        {
            if (await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        /// Prohibits a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task ProhibitPermissionAsync(TRole role, Permission permission)
        {
            if (!await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        /// Prohibits all permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ProhibitAllPermissionsAsync(TRole role)
        {
            foreach (var permission in permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(role, permission);
            }
        }

        /// <summary>
        /// Resets all permission settings for a role.
        /// It removes all permission settings for the role.
        /// Role will have permissions those have <see cref="Permission.IsGrantedByDefault"/> set to true.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ResetAllPermissionsAsync(TRole role)
        {
            await RolePermissionStore.RemoveAllPermissionSettingsAsync(role);
        }

        /// <summary>
        /// Creates a role.
        /// </summary>
        /// <param name="role">Role</param>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            var tenantId = GetCurrentTenantId();
            if (tenantId.HasValue && !role.TenantId.HasValue)
            {
                role.TenantId = tenantId.Value;
            }

            return await base.CreateAsync(role);
        }

        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            return await base.UpdateAsync(role);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role">Role</param>
        public async override Task<IdentityResult> DeleteAsync(TRole role)
        {
            if (role.IsStatic)
            {
                return StudioXIdentityResult.Failed(string.Format(L("CanNotDeleteStaticRole"), role.Name));
            }

            return await base.DeleteAsync(role);
        }

        /// <summary>
        /// Gets a role by given id.
        /// Throws exception if no role with given id.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>Role</returns>
        /// <exception cref="StudioXException">Throws exception if no role with given id</exception>
        public virtual async Task<TRole> GetRoleByIdAsync(int roleId)
        {
            var role = await FindByIdAsync(roleId);
            if (role == null)
            {
                throw new StudioXException("There is no role with id: " + roleId);
            }

            return role;
        }

        /// <summary>
        /// Gets a role by given name.
        /// Throws exception if no role with given roleName.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role</returns>
        /// <exception cref="StudioXException">Throws exception if no role with given roleName</exception>
        public virtual async Task<TRole> GetRoleByNameAsync(string roleName)
        {
            var role = await FindByNameAsync(roleName);
            if (role == null)
            {
                throw new StudioXException("There is no role with name: " + roleName);
            }

            return role;
        }

        public async Task GrantAllPermissionsAsync(TRole role)
        {
            var permissions = permissionManager.GetAllPermissions(role.GetMultiTenancySide());
            await SetGrantedPermissionsAsync(role, permissions);
        }

        [UnitOfWork]
        public virtual async Task<IdentityResult> CreateStaticRoles(int tenantId)
        {
            var staticRoleDefinitions = RoleManagementConfig.StaticRoles.Where(sr => sr.Side == MultiTenancySides.Tenant);

            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var staticRoleDefinition in staticRoleDefinitions)
                {
                    var role = new TRole
                    {
                        TenantId = tenantId,
                        Name = staticRoleDefinition.RoleName,
                        DisplayName = staticRoleDefinition.RoleName,
                        IsStatic = true
                    };

                    var identityResult = await CreateAsync(role);
                    if (!identityResult.Succeeded)
                    {
                        return identityResult;
                    }
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CheckDuplicateRoleNameAsync(int? expectedRoleId, string name, string displayName)
        {
            var role = await FindByNameAsync(name);
            if (role != null && role.Id != expectedRoleId)
            {
                return StudioXIdentityResult.Failed(string.Format(L("RoleNameIsAlreadyTaken"), name));
            }

            role = await FindByDisplayNameAsync(displayName);
            if (role != null && role.Id != expectedRoleId)
            {
                return StudioXIdentityResult.Failed(string.Format(L("RoleDisplayNameIsAlreadyTaken"), displayName));
            }

            return IdentityResult.Success;
        }

        private Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return StudioXStore.FindByDisplayNameAsync(displayName);
        }

        private async Task<RolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return await cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                var newCacheItem = new RolePermissionCacheItem(roleId);

                foreach (var permissionInfo in await RolePermissionStore.GetPermissionsAsync(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.Add(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private string L(string name)
        {
            return LocalizationManager.GetString(StudioXZeroConsts.LocalizationSourceName, name);
        }

        private int? GetCurrentTenantId()
        {
            if (unitOfWorkManager.Current != null)
            {
                return unitOfWorkManager.Current.GetTenantId();
            }

            return StudioXSession.TenantId;
        }
    }
}