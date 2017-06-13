using System;
using System.Linq;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Text;
using StudioX.Web.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace StudioX.AspNetCore.MultiTenancy
{
    public class DomainTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebMultiTenancyConfiguration multiTenancyConfiguration;
        private readonly ITenantStore tenantStore;

        public DomainTenantResolveContributor(
            IHttpContextAccessor httpContextAccessor,
            IWebMultiTenancyConfiguration multiTenancyConfiguration,
            ITenantStore tenantStore)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.multiTenancyConfiguration = multiTenancyConfiguration;
            this.tenantStore = tenantStore;
        }

        public int? ResolveTenantId()
        {
            if (multiTenancyConfiguration.DomainFormat.IsNullOrEmpty())
            {
                return null;
            }

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var hostName = httpContext.Request.Host.Host.RemovePreFix("http://", "https://").RemovePostFix("/");
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