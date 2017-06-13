using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Transactions;
using StudioX.Data;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.MultiTenancy;

namespace StudioX.Zero.EntityFramework
{
    public abstract class StudioXZeroDbMigrator<TDbContext, TConfiguration> : IStudioXZeroDbMigrator, ITransientDependency
        where TDbContext : DbContext
        where TConfiguration : DbMigrationsConfiguration<TDbContext>, IMultiTenantSeed, new()
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IDbPerTenantConnectionStringResolver connectionStringResolver;
        private readonly IIocResolver iocResolver;

        protected StudioXZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager, 
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IIocResolver iocResolver)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.connectionStringResolver = connectionStringResolver;
            this.iocResolver = iocResolver;
        }

        public virtual void CreateOrMigrateForHost()
        {
            CreateOrMigrate(null);
        }

        public virtual void CreateOrMigrateForTenant(StudioXTenantBase tenant)
        {
            if (tenant.ConnectionString.IsNullOrEmpty())
            {
                return;
            }

            CreateOrMigrate(tenant);
        }

        protected virtual void CreateOrMigrate(StudioXTenantBase tenant)
        {
            var args = new DbPerTenantConnectionStringResolveArgs(
                tenant == null ? (int?) null : (int?) tenant.Id,
                tenant == null ? MultiTenancySides.Host : MultiTenancySides.Tenant
                );

            args["DbContextType"] = typeof (TDbContext);
            args["DbContextConcreteType"] = typeof(TDbContext);

            var nameOrConnectionString = ConnectionStringHelper.GetConnectionString(
                connectionStringResolver.GetNameOrConnectionString(args)
            );

            using (var uow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (var dbContext = iocResolver.ResolveAsDisposable<TDbContext>(new {nameOrConnectionString = nameOrConnectionString}))
                {
                    var dbInitializer = new MigrateDatabaseToLatestVersion<TDbContext, TConfiguration>(
                        true,
                        new TConfiguration
                        {
                            Tenant = tenant
                        });

                    dbInitializer.InitializeDatabase(dbContext.Object);

                    unitOfWorkManager.Current.SaveChanges();
                    uow.Complete();
                }
            }
        }
    }
}
