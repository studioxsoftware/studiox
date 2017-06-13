using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Runtime.Security;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Editions;
using StudioX.Boilerplate.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;

namespace StudioX.Boilerplate.MultiTenancy
{
    [StudioXAuthorize(PermissionNames.System.Administration.Tenants.MainMenu)]
    public class TenantAppService : BoilerplateAppServiceBase, ITenantAppService
    {
        private readonly TenantManager tenantManager;
        private readonly RoleManager roleManager;
        private readonly EditionManager editionManager;
        private readonly IStudioXZeroDbMigrator studioXZeroDbMigrator;
        private readonly IPasswordHasher<User> passwordHasher;

        public TenantAppService(
            TenantManager tenantManager, 
            RoleManager roleManager, 
            EditionManager editionManager, 
            IStudioXZeroDbMigrator zeroDbMigrator, 
            IPasswordHasher<User> passwordHasher)
        {
            this.tenantManager = tenantManager;
            this.roleManager = roleManager;
            this.editionManager = editionManager;
            studioXZeroDbMigrator = zeroDbMigrator;
            this.passwordHasher = passwordHasher;
        }

        public ListResultDto<TenantListDto> GetTenants()
        {
            return new ListResultDto<TenantListDto>(
                ObjectMapper.Map<List<TenantListDto>>(
                    tenantManager.Tenants.OrderBy(t => t.TenancyName).ToList()
                )
            );
        }

        public async Task CreateTenant(CreateTenantInput input)
        {
            //Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await TenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new tenant's id.

            //Create tenant database
            studioXZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            //We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                //Create static roles for new tenant
                CheckErrors(await roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); //To get static role ids

                //grant all permissions to admin role
                var adminRole = roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await roleManager.GrantAllPermissionsAsync(adminRole);

                //Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                adminUser.Password = passwordHasher.HashPassword(adminUser, User.DefaultPassword);
                CheckErrors(await UserManager.CreateAsync(adminUser));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get admin user's id

                //Assign admin user to role!
                CheckErrors(await UserManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}