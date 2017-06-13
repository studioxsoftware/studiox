using System;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Runtime;
using Castle.Core.Logging;

namespace StudioX.MultiTenancy
{
    public class TenantResolver : ITenantResolver, ITransientDependency
    {
        private const string AmbientScopeContextKey = "StudioX.MultiTenancy.TenantResolver.Resolving";

        public ILogger Logger { get; set; }

        private readonly IMultiTenancyConfig multiTenancy;
        private readonly IIocResolver iocResolver;
        private readonly ITenantStore tenantStore;
        private readonly ITenantResolverCache cache;
        private readonly IAmbientScopeProvider<bool> ambientScopeProvider;

        public TenantResolver(
            IMultiTenancyConfig multiTenancy,
            IIocResolver iocResolver,
            ITenantStore tenantStore,
            ITenantResolverCache cache,
            IAmbientScopeProvider<bool> ambientScopeProvider)
        {
            this.multiTenancy = multiTenancy;
            this.iocResolver = iocResolver;
            this.tenantStore = tenantStore;
            this.cache = cache;
            this.ambientScopeProvider = ambientScopeProvider;

            Logger = NullLogger.Instance;
        }

        public int? ResolveTenantId()
        {
            if (!multiTenancy.Resolvers.Any())
            {
                return null;
            }

            if (ambientScopeProvider.GetValue(AmbientScopeContextKey))
            {
                //Preventing recursive call of ResolveTenantId
                return null;
            }

            using (ambientScopeProvider.BeginScope(AmbientScopeContextKey, true))
            {
                var cacheItem = cache.Value;
                if (cacheItem != null)
                {
                    return cacheItem.TenantId;
                }

                var tenantId = GetTenantIdFromContributors();
                cache.Value = new TenantResolverCacheItem(tenantId);
                return tenantId;
            }
        }

        private int? GetTenantIdFromContributors()
        {
            foreach (var resolverType in multiTenancy.Resolvers)
            {
                using (var resolver = iocResolver.ResolveAsDisposable<ITenantResolveContributor>(resolverType))
                {
                    int? tenantId;

                    try
                    {
                        tenantId = resolver.Object.ResolveTenantId();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.ToString(), ex);
                        continue;
                    }

                    if (tenantId == null)
                    {
                        continue;
                    }

                    if (tenantStore.Find(tenantId.Value) == null)
                    {
                        continue;
                    }

                    return tenantId;
                }
            }

            return null;
        }
    }
}