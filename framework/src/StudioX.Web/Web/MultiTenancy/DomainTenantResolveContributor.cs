using System;
using System.Linq;
using System.Web;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Text;

namespace StudioX.Web.MultiTenancy
{
    public class DomainTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IWebMultiTenancyConfiguration multiTenancyConfiguration;
        private readonly ITenantStore tenantStore;

        public DomainTenantResolveContributor(
            IWebMultiTenancyConfiguration multiTenancyConfiguration,
            ITenantStore tenantStore)
        {
            this.multiTenancyConfiguration = multiTenancyConfiguration;
            this.tenantStore = tenantStore;
        }

        public int? ResolveTenantId()
        {
            if (multiTenancyConfiguration.DomainFormat.IsNullOrEmpty())
            {
                return null;
            }

            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            var hostName = httpContext.Request.Url.Host.RemovePreFix("http://", "https://").RemovePostFix("/");
            var domainFormat = multiTenancyConfiguration.DomainFormat.RemovePreFix("http://", "https://").Split(':')[0].RemovePostFix("/");
            var result = new FormattedStringValueExtracter().Extract(hostName, domainFormat, true);
            if (!result.IsMatch || !result.Matches.Any())
            {
                return null;
            }

            var tenancyName = result.Matches[0].Value;
            if (tenancyName.IsNullOrEmpty())
            {
                return null;
            }

            if (string.Equals(tenancyName, "www", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var tenantInfo = tenantStore.Find(tenancyName);
            if (tenantInfo == null)
            {
                return null;
            }

            return tenantInfo.Id;
        }
    }
}