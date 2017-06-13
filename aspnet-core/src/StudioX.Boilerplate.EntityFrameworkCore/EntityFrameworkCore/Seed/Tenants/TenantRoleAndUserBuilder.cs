using System.Linq;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StudioX.Boilerplate.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly BoilerplateDbContext context;
        private readonly int tenantId;

        public TenantRoleAndUserBuilder(BoilerplateDbContext context, int tenantId)
        {
            this.context = context;
            this.tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            //Admin role

            var adminRole = context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = context.Roles.Add(new Role(tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                context.SaveChanges();

                //Grant all permissions to admin role
                var permissions = PermissionFinder
                    .GetAllPermissions(new BoilerplateAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant))
                    .ToList();

                foreach (var permission in permissions)
                {
                    context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            TenantId = tenantId,
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRole.Id
                        });
                }

                context.SaveChanges();
            }

            //admin user

            var adminUser = context.Users.FirstOrDefault(u => u.TenantId == tenantId && u.UserName == StudioXUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                context.Users.Add(adminUser);
                context.SaveChanges();

                //Assign Admin role to admin user
                context.UserRoles.Add(new UserRole(tenantId, adminUser.Id, adminRole.Id));
                context.SaveChanges();

                //User account of admin user
                if (tenantId == 1)
                {
                    context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = tenantId,
                        UserId = adminUser.Id,
                        UserName = StudioXUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
