using System.Collections.Generic;
using System.Linq;
using StudioX.MultiTenancy;

namespace StudioX.AspNetCore.App.MultiTenancy
{
    public class TestTenantStore : ITenantStore
    {
        private readonly List<TenantInfo> tenants = new List<TenantInfo>
        {
            new TenantInfo(1, "Default"),
            new TenantInfo(42, "acme"),
            new TenantInfo(43, "vlsft")
        };

        public TenantInfo Find(int tenantId)
        {
            return tenants.FirstOrDefault(t => t.Id == tenantId);
        }

        public TenantInfo Find(string tenancyName)
        {
            return tenants.FirstOrDefault(t => t.TenancyName.ToLower() == tenancyName.ToLower());
        }
    }
}
