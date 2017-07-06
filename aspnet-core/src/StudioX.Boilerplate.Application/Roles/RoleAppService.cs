using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.MultiTenancy;
using StudioX.Boilerplate.Roles.Dto;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.IdentityFramework;
using StudioX.Linq.Extensions;
using StudioX.UI;
using StudioX.Zero.Configuration;

namespace StudioX.Boilerplate.Roles
{
    [StudioXAuthorize(PermissionNames.System.Administration.Roles.MainMenu)]
    public class RoleAppService :
        AsyncCrudAppService<Role, RoleDto, int, PagedResultRequestDto, CreateRoleInput, UpdateRoleInput>,
        IRoleAppService

    {
        private readonly RoleManager roleManager;
        private UserManager userManager;
        private TenantManager tenantManager;

        public RoleAppService(IRepository<Role> roleRepository,
            RoleManager roleManager,
            UserManager userManager)
            : base(roleRepository)
        {
            this.roleManager = roleManager;

            GetAllPermissionName = PermissionNames.System.Administration.Roles.MainMenu;
            GetPermissionName = PermissionNames.System.Administration.Roles.MainMenu;
            CreatePermissionName = PermissionNames.System.Administration.Roles.Create;
            UpdatePermissionName = PermissionNames.System.Administration.Roles.Edit;
            DeletePermissionName = PermissionNames.System.Administration.Roles.Delete;
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Permissions);
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await CreateFilteredQuery(null)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        // Sorting has to be re-applied after paging, .Take resets the sorting
        protected override IQueryable<Role> ApplyPaging(IQueryable<Role> query, PagedResultRequestDto input)
        {
            //Try to use paging if available
            var pagedInput = input as IPagedResultRequest;
            if (pagedInput != null)
            {
                // Sort again after paging // .take resets the orderby
                query = query.PageBy(pagedInput);
                query = ApplySorting(query, input);
                return query;
            }

            // Try to limit query result if available
            var limitedInput = input as ILimitedResultRequest;
            if (limitedInput != null)
            {
                query = query.Take(limitedInput.MaxResultCount);
                query = ApplySorting(query, input);
                return query;
            }

            // No paging
            return query;
        }

        [UnitOfWork]
        public override async Task<RoleDto> Create(CreateRoleInput input)
        {
            CheckCreatePermission();

            var role = ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            var result = await roleManager.CreateAsync(role);
            CheckErrors(result);

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        [UnitOfWork]
        public override async Task<RoleDto> Update(UpdateRoleInput input)
        {
            CheckUpdatePermission();

            var role = await roleManager.GetRoleByIdAsync(input.Id);

            // Update role
            var mapped = input.MapTo(role);

            CheckErrors(await roleManager.UpdateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            // should reload entity fresh from db after changes
            return MapToEntityDto(role);
        }

        [UnitOfWork]
        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await roleManager.FindByIdAsync(input.Id.ToString());
            if (role.IsStatic)
            {
                throw new UserFriendlyException("CannotDeleteAStaticRole");
            }

            var users = await userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            var result = await roleManager.DeleteAsync(role);


            CheckErrors(result);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
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

            var tenants = tenantManager.Tenants.ToList();

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