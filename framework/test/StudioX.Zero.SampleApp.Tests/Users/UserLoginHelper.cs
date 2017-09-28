using System.Linq;
using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserLoginHelper
    {
        public static void CreateTestUsers(AppDbContext context)
        {
            var defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);

            context.Users.Add(
                new User
                {
                    UserName = "userOwner",
                    FirstName = "Owner",
                    LastName = "One",
                    EmailAddress = "owner@studioxsoftware.com",
                    IsEmailConfirmed = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                });

            context.Users.Add(
                new User
                {
                    TenantId = defaultTenant.Id, //A user of tenant1
                    UserName = "user1",
                    FirstName = "User",
                    LastName = "One",
                    EmailAddress = "user-one@studioxsoftware.com",
                    IsEmailConfirmed = false,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                });
        }
    }
}