using StudioX.Zero.EntityFrameworkCore;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Boilerplate.EntityFrameworkCore
{
    public class BoilerplateDbContext : StudioXZeroDbContext<Tenant, Role, User, BoilerplateDbContext>
    {
        /* Define an IDbSet for each entity of the application */
        
        public BoilerplateDbContext(DbContextOptions<BoilerplateDbContext> options)
            : base(options)
        {

        }
    }
}
