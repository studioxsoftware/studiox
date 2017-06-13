using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

using StudioX.Dapper.Tests.Entities;
using StudioX.EntityFramework;

namespace StudioX.Dapper.Tests
{
    [DbConfigurationType(typeof(DapperDbContextConfiguration))]
    public class SampleDapperApplicationDbContext : StudioXDbContext
    {
        public SampleDapperApplicationDbContext(DbConnection connection)
            : base(connection, false)
        {
        }

        public SampleDapperApplicationDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public virtual IDbSet<Product> Products { get; set; }
    }

    public class DapperDbContextConfiguration : DbConfiguration
    {
        public DapperDbContextConfiguration()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                SqlProviderServices.Instance
            );
        }
    }
}
