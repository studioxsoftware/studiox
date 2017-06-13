using System;
using System.Transactions;
using StudioX.Data;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.EntityFrameworkCore
{
    public abstract class StudioXZeroDbMigrator<TDbContext> : IStudioXZeroDbMigrator, ITransientDependency
        where TDbContext : DbContext
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IDbPerTenantConnectionStringResolver connectionStringResolver;
        private readonly IDbContextResolver dbContextResolver;

        protected StudioXZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.connectionStringResolver = connectionStringResolver;
            this.dbContextResolver = dbContextResolver;
        }
        
        public virtual void CreateOrMigrateForHost()
        {
            CreateOrMigrateForHost(null);
        }

        public virtual void CreateOrMigrateForHost(Action<TDbContext> seedAction)
        {
            CreateOrMigrate(null, seedAction);
        }

        public virtual void CreateOrMigrateForTenant(StudioXTenantBase tenant)
        {
            CreateOrMigrateForTenant(tenant, null);
        }

        public virtual void CreateOrMigrateForTenant(StudioXTenantBase tenant, Action<TDbContext> seedAction)
        {
            if (tenant.ConnectionString.IsNullOrEmpty())
            {
                return;
            }

            CreateOrMigrate(tenant, seedAction);
        }

        protected virtual void CreateOrMigrate(StudioXTenantBase tenant, Action<TDbContext> seedAction)
        {
            var args = new DbPerTenantConnectionStringResolveArgs(
                tenant == null ? (int?) null : (int?) tenant.Id,
                tenant == null ? MultiTenancySides.Host : MultiTenancySides.Tenant
            )
            {
                ["DbContextType"] = typeof(TDbContext),
                ["DbContextConcreteType"] = typeof(TDbContext)
            };


            var nameOrConnectionString = ConnectionStringHelper.GetConnectionString(
                connectionStringResolver.GetNameOrConnectionString(args)
            );

            using (var uow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (var dbContext = dbContextResolver.Resolve<TDbContext>(nameOrConnectionString, null))
                {
                    dbContext.Database.Migrate();
                    seedAction?.Invoke(dbContext);
                    unitOfWorkManager.Current.SaveChanges();
                    uow.Complete();
                }
            }
        }
    }
}
