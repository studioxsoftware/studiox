using System.Linq;
using StudioX.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas
{
    public class HostUserBuilder
    {
        private readonly AppDbContext context;

        public HostUserBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            CreateUsers();
        }

        private void CreateUsers()
        {
            var adminUser = context.Users.FirstOrDefault(u => u.UserName == User.AdminUserName);

            if (adminUser == null)
            {
                adminUser = context.Users.Add(
                    new User
                    {
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