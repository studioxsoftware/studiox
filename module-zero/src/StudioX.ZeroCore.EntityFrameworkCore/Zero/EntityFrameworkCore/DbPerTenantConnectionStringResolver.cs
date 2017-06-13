using StudioX.Configuration.Startup;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Runtime.Session;

namespace StudioX.Zero.EntityFrameworkCore
{
    /// <summary>
    /// Implements <see cref="IDbPerTenantConnectionStringResolver"/> to dynamically resolve
    /// connection string for a multi tenant application.
    /// </summary>
    public class DbPerTenantConnectionStringResolver : DefaultConnectionStringResolver, IDbPerTenantConnectionStringResolver
    {
        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        private readonly ICurrentUnitOfWorkProvider currentUnitOfWorkProvider;
        private readonly ITenantCache tenantCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbPerTenantConnectionStringResolver"/> class.
        /// </summary>
        public DbPerTenantConnectionStringResolver(
            IStudioXStartupConfiguration configuration,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            ITenantCache tenantCache)
            : base(configuration)
        {
            this.currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            this.tenantCache = tenantCache;

            StudioXSession = NullStudioXSession.Instance;
        }

        public override string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            if (args.MultiTenancySide == MultiTenancySides.Host)
            {
                return GetNameOrConnectionString(new DbPerTenantConnectionStringResolveArgs(null, args));
            }

            return GetNameOrConnectionString(new DbPerTenantConnectionStringResolveArgs(GetCurrentTenantId(), args));
        }

        public virtual string GetNameOrConnectionString(DbPerTenantConnectionStringResolveArgs args)
        {
            if (args.TenantId == null)
            {
                //Requested for host
                return base.GetNameOrConnectionString(args);
            }

            var tenantCacheItem = tenantCache.Get(args.TenantId.Value);
            if (tenantCacheItem.ConnectionString.IsNullOrEmpty())
            {
                //Tenant has not dedicated database
                return base.GetNameOrConnectionString(args);
            }

            return tenantCacheItem.ConnectionString;
        }

        protected virtual int? GetCurrentTenantId()
        {
            return currentUnitOfWorkProvider.Current != null
                ? currentUnitOfWorkProvider.Current.GetTenantId()
                : StudioXSession.TenantId;
        }
    }
}
