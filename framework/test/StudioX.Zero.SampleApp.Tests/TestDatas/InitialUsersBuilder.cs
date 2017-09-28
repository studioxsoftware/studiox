using System.Linq;
using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    public class InitialUsersBuilder
    {
        private readonly AppDbContext context;

        public InitialUsersBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            CreateUsers();
        }

        private void CreateUsers()
        {
            var defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);

            var admin = context.Users.Add(
                new User
                {
                    TenantId = defaultTenant.Id,
                    FirstName = "System",
                    LastName = "Administrator",
                    UserName = User.AdminUserName,
                    Password = new PasswordHasher().HashPassword("123qwe"),
                    EmailAddress = "admin@studioxsoftware.com"
                });

            context.Users.Add(
                new User
                {
                    TenantId = defaultTenant.Id,
                    FirstName = "System",
                    LastName = "Manager",
                    UserName = "manager",
                    Password = new PasswordHasher().HashPassword("123qwe"),
                    EmailAddress = "manager@studioxsoftware.com"
                });
        }
    }
}