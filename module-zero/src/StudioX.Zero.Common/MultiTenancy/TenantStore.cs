namespace StudioX.MultiTenancy
{
    public class TenantStore : ITenantStore
    {
        private readonly ITenantCache tenantCache;

        public TenantStore(ITenantCache tenantCache)
        {
            this.tenantCache = tenantCache;
        }

        public TenantInfo Find(int tenantId)
        {
            var tenant = tenantCache.GetOrNull(tenantId);
            return tenant == null ? null : new TenantInfo(tenant.Id, tenant.TenancyName);
        }

        public TenantInfo Find(string tenancyName)
        {
            var tenant = tenantCache.GetOrNull(tenancyName);
            return tenant == null ? null : new TenantInfo(tenant.Id, tenant.TenancyName);
        }
    }
}
