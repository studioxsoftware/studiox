using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Roles
{
    /// <summary>
    /// Implements 'Role Store' of ASP.NET Identity Framework.
    /// </summary>
    public abstract class StudioXRoleStore<TRole, TUser> :
        IQueryableRoleStore<TRole, int>,
        IRolePermissionStore<TRole>,

        ITransientDependency

        where TRole : StudioXRole<TUser>
        where TUser : StudioXUser<TUser>
    {
        private readonly IRepository<TRole> roleRepository;
        private readonly IRepository<UserRole, long> userRoleRepository;
        private readonly IRepository<RolePermissionSetting, long> rolePermissionSettingRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected StudioXRoleStore(
            IRepository<TRole> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
        {
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.rolePermissionSettingRepository = rolePermissionSettingRepository;
        }

        public virtual IQueryable<TRole> Roles => roleRepository.GetAll();

        public virtual async Task CreateAsync(TRole role)
        {
            await roleRepository.InsertAsync(role);
        }

        public virtual async Task UpdateAsync(TRole role)
        {
            await roleRepository.UpdateAsync(role);
        }

        public virtual async Task DeleteAsync(TRole role)
        {
            await userRoleRepository.DeleteAsync(ur => ur.RoleId == role.Id);
            await roleRepository.DeleteAsync(role);
        }

        public virtual async Task<TRole> FindByIdAsync(int roleId)
        {
            return await roleRepository.FirstOrDefaultAsync(roleId);
        }

        public virtual async Task<TRole> FindByNameAsync(string roleName)
        {
            return await roleRepository.FirstOrDefaultAsync(
                role => role.Name == roleName
                );
        }

        public virtual async Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return await roleRepository.FirstOrDefaultAsync(
                role => role.DisplayName == displayName
                );
        }

        /// <inheritdoc/>
        public virtual async Task AddPermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(role.Id, permissionGrant))
            {
                return;
            }

            await rolePermissionSettingRepository.InsertAsync(
                new RolePermissionSetting
                {
                    TenantId = role.TenantId,
                    RoleId = role.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        /// <inheritdoc/>
        public virtual async Task RemovePermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            await rolePermissionSettingRepository.DeleteAsync(
                permissionSetting => permissionSetting.RoleId == role.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
                );
        }

        /// <inheritdoc/>
        public virtual Task<IList<PermissionGrantInfo>> GetPermissionsAsync(TRole role)
        {
            return GetPermissionsAsync(role.Id);
        }

        public async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int roleId)
        {
            return (await rolePermissionSettingRepository.GetAllListAsync(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        /// <inheritdoc/>
        public virtual async Task<bool> HasPermissionAsync(int roleId, PermissionGrantInfo permissionGrant)
        {
            return await rolePermissionSettingRepository.FirstOrDefaultAsync(
                p => p.RoleId == roleId &&
                     p.Name == permissionGrant.Name &&
                     p.IsGranted == permissionGrant.IsGranted
                ) != null;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAllPermissionSettingsAsync(TRole role)
        {
            await rolePermissionSettingRepository.DeleteAsync(s => s.RoleId == role.Id);
        }

        public virtual void Dispose()
        {
            //No need to dispose since using IOC.
        }
    }
}
