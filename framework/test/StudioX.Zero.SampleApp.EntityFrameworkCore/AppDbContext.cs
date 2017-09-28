using StudioX.Zero.EntityFrameworkCore;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore
{
    public class AppDbContext : StudioXZeroDbContext<Tenant, Role, User, AppDbContext>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {

        }
    }
}