using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using StudioX.Dapper.Tests.Entities;
using StudioX.EntityFramework;

namespace StudioX.Dapper.Tests
{
    [DbConfigurationType(typeof(DapperDbContextConfiguration2))]
    public class SampleDapperApplicationDbContext2 : StudioXDbContext
    {
        public SampleDapperApplicationDbContext2(DbConnection connection)
            : base(connection, false)
        {
        }

        public SampleDapperApplicationDbContext2(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public virtual IDbSet<ProductDetail> ProductDetails { get; set; }
    }

    public class DapperDbContextConfiguration2 : DbConfiguration
    {
        public DapperDbContextConfiguration2()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}