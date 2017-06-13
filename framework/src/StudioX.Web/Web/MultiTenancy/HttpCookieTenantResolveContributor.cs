using System.Web;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.MultiTenancy;

namespace StudioX.Web.MultiTenancy
{
    public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        public int? ResolveTenantId()
        {
            var cookie = HttpContext.Current?.Request.Cookies[MultiTenancyConsts.TenantIdResolveKey];
            if (cookie == null || cookie.Value.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(cookie.Value, out tenantId) ? (int?) null : tenantId;
        }
    }
}