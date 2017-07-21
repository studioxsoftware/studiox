using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Editions;
using StudioX.Boilerplate.MultiTenancy.Dto;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.IdentityFramework;
using StudioX.MultiTenancy;
using StudioX.Runtime.Security;
using StudioX.UI;

namespace StudioX.Boilerplate.MultiTenancy
{
    public class TenantAppService :
        AsyncCrudAppService<Tenant, TenantDto, int, PagedResultRequestDto, CreateTenantInput, UpdateTenantInput>,
        ITenantAppService
    {
        private readonly TenantManager tenantManager;
        private readonly EditionManager editionManager;
        private readonly RoleManager roleManager;
        private readonly UserManager userManager;
        private readonly IStudioXZeroDbMigrator studioXZeroDbMigrator;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IRepository<User, long> userRepository;

        public TenantAppService(
            IRepository<Tenant, int> service,
            IRepository<User, long> userRepository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IStudioXZeroDbMigrator studioXZeroDbMigrator,
            IPasswordHasher<User> passwordHasher
        ) : base(service)
        {
            this.tenantManager = tenantManager;
            this.editionManager = editionManager;
            this.roleManager = roleManager;
            this.studioXZeroDbMigrator = studioXZeroDbMigrator;
            this.passwordHasher = passwordHasher;
            this.userManager = userManager;
            this.userRepository = userRepository;

            GetAllPermissionName = PermissionNames.System.Administration.Tenants.MainMenu;
            GetPermissionName = PermissionNames.System.Administration.Tenants.MainMenu;
            CreatePermissionName = PermissionNames.System.Administration.Tenants.Create;
            UpdatePermissionName = PermissionNames.System.Administration.Tenants.Edit;
            DeletePermissionName = PermissionNames.System.Administration.Tenants.Delete;
        }

        public override async Task<TenantDto> Get(EntityDto<int> input)
        {
            CheckGetPermission();

            var tenantDto = await base.Get(input);

            Role tenantAdminRole = null;
            User tenantAdminUser = null;
            using (CurrentUnitOfWork.SetTenantId(tenantDto.Id))
            {
                tenantAdminRole = await roleManager.GetRoleByNameAsync(StaticRoleNames.Tenants.Admin);
                tenantAdminUser = userRepository.GetAllIncluding(x => x.Roles)
                    .Single(y => y.Roles.Any(z => z.RoleId == tenantAdminRole.Id));
            }

            tenantDto.AdminEmailAddress = tenantAdminUser.EmailAddress;

            return tenantDto;
        }

        [UnitOfWork]
        public override async Task<TenantDto> Create(CreateTenantInput input)
        {
            CheckCreatePermission();

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

            await tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new tenant's id.

            //Create tenant database
            studioXZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            //We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Check directly
                var isGranted =
                    await PermissionChecker.IsGrantedAsync(PermissionNames.System.Administration.Tenants.MainMenu);

                //Create static roles for new tenant
                CheckErrors(await roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); //To get static role ids

                //grant all permissions to admin role
                var adminRole = roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await roleManager.GrantAllPermissionsAsync(adminRole);

                //Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                adminUser.Password = passwordHasher.HashPassword(adminUser, User.DefaultPassword);
                CheckErrors(await userManager.CreateAsync(adminUser));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get admin user's id

                //Assign admin user to role!
                CheckErrors(await userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return MapToEntityDto(tenant);
        }

        [UnitOfWork]
        public override async Task<TenantDto> Update(UpdateTenantInput input)
        {
            CheckUpdatePermission();

            var tenant = tenantManager.GetById(input.Id);

            Role tenantAdminRole = null;
            User tenantAdminUser = null;
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                tenantAdminRole = await roleManager.GetRoleByNameAsync("Admin");
                tenantAdminUser = userRepository.GetAll()
                    .FirstOrDefault(x => x.Roles
                        .Any(y => y.RoleId == tenantAdminRole.Id)
                    );
            }

            // Update the admin email address
            if (tenantAdminUser != null && input.AdminEmailAddress != tenantAdminUser.EmailAddress)
            {
                User existingUser = null;
                using (CurrentUnitOfWork.SetTenantId(tenant.Id))
                {
                    existingUser = userRepository.GetAll()
                        .FirstOrDefault(x => x.EmailAddress == input.AdminEmailAddress);
                }

                if (existingUser != null)
                {
                    throw new UserFriendlyException("There is an existing user for the tenant with email address " +
                                                    input.AdminEmailAddress);
                }

                tenantAdminUser.EmailAddress = input.AdminEmailAddress;
                tenantAdminUser.SetNormalizedNames();
            }

            var connectionString = tenant.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Decrypt(tenant.ConnectionString);
            if (connectionString != input.ConnectionString)
            {
                throw new UserFriendlyException("Changing Connection string is not supported.");
            }

            var mapped = input.MapTo(tenant);
            await tenantManager.UpdateAsync(mapped);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(tenant);
        }

        [UnitOfWork]
        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await tenantManager.GetByIdAsync(input.Id);
            await tenantManager.DeleteAsync(tenant);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}