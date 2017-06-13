using System.Linq;
using StudioX.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas
{
    public class TenantUserBuilder
    {
        private readonly AppDbContext context;

        public TenantUserBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build(int tenantId)
        {
            CreateUsers(tenantId);
        }

        private void CreateUsers(int tenantId)
        {
            var adminUser = context.Users.FirstOrDefault(u => u.TenantId == tenantId && u.UserName == User.AdminUserName);

            if (adminUser == null)
            {
                adminUser = context.Users.Add(
                    new User
                    {
                        TenantId = tenantId,
                        FirstName = "System",
                        LastName = "Administrator",
                        UserName = User.AdminUserName,
                        Password = new PasswordHasher().HashPassword("123qwe"),
                        EmailAddress = "admin@studioxsoftware.com"
                    }).Entity;

                context.SaveChanges();
            }
        }
    }
}