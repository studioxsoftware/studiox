using System.Web;
using StudioX.Dependency;
using StudioX.MultiTenancy;

namespace StudioX.Web.MultiTenancy
{
    public class HttpContextTenantResolverCache : ITenantResolverCache, ITransientDependency
    {
        private const string CacheItemKey = "StudioX.MultiTenancy.TenantResolverCacheItem";

        public TenantResolverCacheItem Value
        {
            get
            {
                return HttpContext.Current?.Items[CacheItemKey] as TenantResolverCacheItem;
            }

            set
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                {
                    return;
                }

                httpContext.Items[CacheItemKey] = value;
            }
        }
    }
}
