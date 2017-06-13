using System.Linq;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.ZeroCore.SampleApp.Application;
using StudioX.ZeroCore.SampleApp.Core;

namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly SampleAppDbContext context;

        public HostRoleAndUserCreator(SampleAppDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = context.Roles.FirstOrDefault(r => r.TenantId == null && r.Name == AppStaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = context.Roles.Add(new Role(null, AppStaticRoleNames.Host.Admin, AppStaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                context.SaveChanges();
            }

            //admin user for host

            var adminUserForHost = context.Users.FirstOrDefault(u => u.TenantId == null && u.UserName == StudioXUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = StudioXUserBase.AdminUserName,
                    FirstName = "admin",
                    LastName = "admin",
                    EmailAddress = "admin@aspnetzero.com",
                    IsEmailConfirmed = true,
                    IsActive = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                };

                user.SetNormalizedNames();

                adminUserForHost = context.Users.Add(user).Entity;
                context.SaveChanges();

                //Assign Admin role to admin user
                context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                context.SaveChanges();

                //Grant all permissions
                var permissions = PermissionFinder
                    .GetAllPermissions(new AppAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host))
                    .ToList();

                foreach (var permission in permissions)
                {
                    context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            TenantId = null,
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRoleForHost.Id
                        });
                }

                context.SaveChanges();

                //User account of admin user
                context.UserAccounts.Add(new UserAccount
                {
                    TenantId = null,
                    UserId = adminUserForHost.Id,
                    UserName = StudioXUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress
                });

                context.SaveChanges();
            }
        }
    }
}