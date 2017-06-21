using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.AutoMapper;
using StudioX.Configuration;
using StudioX.Extensions;
using StudioX.Linq.Extensions;
using StudioX.UI;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Roles.Dto;
using StudioX.Zero.Configuration;

namespace StudioX.Boilerplate.Roles
{
    [StudioXAuthorize(PermissionNames.System.Administration.Roles.MainMenu)]
    public class RoleAppService : BoilerplateAppServiceBase, IRoleAppService
    {
        private readonly RoleManager roleManager;

        public RoleAppService(
            RoleManager roleManager
        )
        {
            this.roleManager = roleManager;
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.View)]
        public async Task<ListResultDto<RoleListDto>> GetAll()
        {
            var roles = roleManager.Roles
                .OrderByDescending(r => r.IsStatic)
                .ThenByDescending(r => r.IsDefault)
                .ThenBy(r => r.DisplayName)
                .ToList();

            return new ListResultDto<RoleListDto>(
                   ObjectMapper.Map<List<RoleListDto>>(roles)
               );
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.View)]
        public PagedResultDto<RoleListDto> PagedResult(GetRolesInput input)
        {
            if (input.MaxResultCount <= 0)
                input.MaxResultCount = SettingManager.GetSettingValue<int>(BoilerplateSettingProvider.RolesDefaultPageSize);

            if (input.Sorting.IsNullOrEmpty())
                input.Sorting = GetRolesInput.DefaultSorting;

            var query = roleManager.Roles
                .WhereIf(!input.PermissionName.IsNullOrEmpty(),
                    x => x.Permissions.Any(p => p.Name == input.PermissionName));

            var roles = query.OrderBy(input.Sorting)
                .PageBy(input)
                .ToList();
            var totalCount = query.Count();

            return new PagedResultDto<RoleListDto>
            {
                TotalCount = totalCount,
                Items = roles.MapTo<List<RoleListDto>>()
            };
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.View)]
        public async Task<RoleDto> GetDefaultRole()
        {
            var defaultRole = await roleManager.Roles.FirstOrDefaultAsync(r => r.IsDefault);
            return defaultRole.MapTo<RoleDto>();
        }
        
        [StudioXAuthorize(PermissionNames.System.Administration.Roles.View)]
        public async Task<RoleDto> Get(int id)
        {
            var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
            return role.MapTo<RoleDto>();
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.Create)]
        public async Task Create(CreateRoleInput input)
        {
            if (input.IsDefault)
            {
                var defaultRoles = roleManager.Roles.Where(r => r.IsDefault);
                foreach (var defaultRole in defaultRoles)
                    defaultRole.IsDefault = false;
            }

            var role = new Role
            {
                Name = new RegularGuidGenerator().Create().ToString().Replace("-", ""),
                DisplayName = input.DisplayName,
                IsDefault = input.IsDefault
            };

            await roleManager.CreateAsync(role);
            await CurrentUnitOfWork.SaveChangesAsync();

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.Edit)]
        public async Task Update(UpdateRoleInput input)
        {
            var role = await roleManager.GetRoleByIdAsync(input.Id);

            var mapped = input.MapTo(role);

            await roleManager.UpdateAsync(mapped);
            await roleManager.ResetAllPermissionsAsync(role);

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Roles.Delete)]
        public async Task Delete(int id)
        {
            var role = await roleManager.GetRoleByIdAsync(id);
            if (role != null && !role.IsStatic)
            {
                await roleManager.ResetAllPermissionsAsync(role);
                await roleManager.DeleteAsync(role);
            }
        }

        // TODO: Will change to other permission name
        [StudioXAuthorize(PermissionNames.System.Administration.Roles.Edit)]
        public async Task GrantAllPermissionsForHost(string password)
        {
            await CheckPassword(password);

            // Grant all permissions to admin role
            var adminRole = roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
            await roleManager.GrantAllPermissionsAsync(adminRole);
        }

        // TODO: Will change to other permission name
        [StudioXAuthorize(PermissionNames.System.Administration.Roles.Edit)]
        public async Task GrantAllPermissionsForAllTenant(string password)
        {
            await CheckPassword(password);

            var tenants = TenantManager.Tenants.ToList();

            foreach (var tenant in tenants)
            {
                using (CurrentUnitOfWork.SetTenantId(tenant.Id))
                {
                    // Grant all permissions to admin role
                    var adminRole = roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await roleManager.GrantAllPermissionsAsync(adminRole);
                }
            }
        }

        private async Task CheckPassword(string password)
        {
            if (password.IsNullOrEmpty())
                throw new UserFriendlyException("Password can not be null or empty!");

            var actualPassword = await SettingManager.GetSettingValueAsync(
                    StudioXZeroSettingNames.UserManagement.SecurityPassword.MaintainanceAdminPassword);

            if (actualPassword != password)
                throw new UserFriendlyException("Password is not correct!");
        }
    }
}