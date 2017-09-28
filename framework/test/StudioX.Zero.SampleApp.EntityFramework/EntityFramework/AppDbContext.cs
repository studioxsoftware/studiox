using System.Data.Common;
using StudioX.Zero.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.EntityFramework
{
    public class AppDbContext : StudioXZeroDbContext<Tenant, Role, User>
    {
        public AppDbContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {

        }
    }
}