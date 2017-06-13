using System.Linq;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.ZeroCore.SampleApp.Application;
using StudioX.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly SampleAppDbContext context;
        private readonly int tenantId;

        public TenantRoleAndUserBuilder(SampleAppDbContext context, int tenantId)
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

            var adminRole = context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == AppStaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = context.Roles.Add(new Role(tenantId, AppStaticRoleNames.Tenants.Admin, AppStaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                context.SaveChanges();

                //Grant all permissions to admin role
                var permissions = PermissionFinder
                    .GetAllPermissions(new AppAuthorizationProvider())
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

            //User role

            var userRole = context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == AppStaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                context.Roles.Add(new Role(tenantId, AppStaticRoleNames.Tenants.User, AppStaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
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
