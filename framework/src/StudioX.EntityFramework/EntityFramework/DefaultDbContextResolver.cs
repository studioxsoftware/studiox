using System;
using System.Data.Common;
using System.Data.Entity;
using StudioX.Dependency;

namespace StudioX.EntityFramework
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IDbContextTypeMatcher dbContextTypeMatcher;

        public DefaultDbContextResolver(IIocResolver iocResolver, IDbContextTypeMatcher dbContextTypeMatcher)
        {
            this.iocResolver = iocResolver;
            this.dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString)
            where TDbContext : DbContext
        {
            var dbContextType = GetConcreteType<TDbContext>();
            return (TDbContext) iocResolver.Resolve(dbContextType, new
            {
                nameOrConnectionString = connectionString
            });
        }

        public TDbContext Resolve<TDbContext>(DbConnection existingConnection, bool contextOwnsConnection)
            where TDbContext : DbContext
        {
            var dbContextType = GetConcreteType<TDbContext>();
            return (TDbContext) iocResolver.Resolve(dbContextType, new
            {
                existingConnection,
                contextOwnsConnection
            });
        }

        protected virtual Type GetConcreteType<TDbContext>()
        {
            var dbContextType = typeof(TDbContext);
            return !dbContextType.IsAbstract
                ? dbContextType
                : dbContextTypeMatcher.GetConcreteType(dbContextType);
        }
    }
}