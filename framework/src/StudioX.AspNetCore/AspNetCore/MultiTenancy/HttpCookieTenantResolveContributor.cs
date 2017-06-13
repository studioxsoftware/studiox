using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace StudioX.AspNetCore.MultiTenancy
{
    public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpCookieTenantResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int? ResolveTenantId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdValue = httpContext.Request.Cookies[MultiTenancyConsts.TenantIdResolveKey];
            if (tenantIdValue.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(tenantIdValue, out tenantId) ? (int?) null : tenantId;
        }
    }
}