using System;
using System.Security.Claims;
using StudioX.Runtime.Security;

namespace StudioX.Authorization
{
    internal static class StudioXZeroClaimsIdentityHelper
    {
        public static int? GetTenantId(ClaimsPrincipal principal)
        {
            var tenantIdOrNull = principal?.FindFirstValue(StudioXClaimTypes.TenantId);
            if (tenantIdOrNull == null)
            {
                return null;
            }

            return Convert.ToInt32(tenantIdOrNull);
        }
    }
}