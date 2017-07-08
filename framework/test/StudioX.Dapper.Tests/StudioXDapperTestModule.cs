using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using StudioX.EntityFramework;
using StudioX.Modules;
using StudioX.TestBase;

using DapperExtensions.Sql;

namespace StudioX.Dapper.Tests
{
    [DependsOn(
        typeof(StudioXEntityFrameworkModule),
        typeof(StudioXTestBaseModule),
        typeof(StudioXDapperModule)
    )]
    public class StudioXDapperTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { Assembly.GetExecutingAssembly() });
        }
    }
}
