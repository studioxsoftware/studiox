using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;

namespace StudioX.Zero.EntityFramework
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class StudioXZeroTenantDbContext<TRole, TUser> : StudioXZeroCommonDbContext<TRole, TUser>
        where TRole : StudioXRole<TUser>
        where TUser : StudioXUser<TUser>
    {
        /// <summary>
        /// Default constructor.
        /// Do not directly instantiate this class. Instead, use dependency injection!
        /// </summary>
        protected StudioXZeroTenantDbContext()
        {

        }

        /// <summary>
        /// Constructor with connection string parameter.
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or a name in connection strings in configuration file</param>
        protected StudioXZeroTenantDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected StudioXZeroTenantDbContext(DbCompiledModel model)
            : base(model)
        {

        }

        /// <summary>
        /// This constructor can be used for unit tests.
        /// </summary>
        protected StudioXZeroTenantDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected StudioXZeroTenantDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        protected StudioXZeroTenantDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        protected StudioXZeroTenantDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }
    }
}