using System.Linq;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.MultiTenancy;

namespace StudioX.AspNetCore.MultiTenancy
{
    public class HttpHeaderTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpHeaderTenantResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

            Logger = NullLogger.Instance;
        }

        public int? ResolveTenantId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdHeader = httpContext.Request.Headers[MultiTenancyConsts.TenantIdResolveKey];
            if (tenantIdHeader == string.Empty || tenantIdHeader.Count < 1)
            {
                return null;
            }

            if (tenantIdHeader.Count > 1)
            {
                Logger.Warn(
                    $"HTTP request includes more than one {MultiTenancyConsts.TenantIdResolveKey} header value. First one will be used. All of them: {tenantIdHeader.JoinAsString(", ")}"
                );
            }

            int tenantId;
            return !int.TryParse(tenantIdHeader.First(), out tenantId) ? (int?) null : tenantId;
        }
    }
}