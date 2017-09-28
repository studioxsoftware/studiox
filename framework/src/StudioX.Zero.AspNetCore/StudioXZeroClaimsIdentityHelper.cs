using System;
using System.Security.Claims;
using System.Security.Principal;
using StudioX.Runtime.Security;
using Microsoft.AspNet.Identity;

namespace StudioX.Zero.AspNetCore
{
    internal static class StudioXZeroClaimsIdentityHelper
    {
        public static int? GetTenantId(IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            var claimsIdentity = identity as ClaimsIdentity;

            var tenantIdOrNull = claimsIdentity?.FindFirstValue(StudioXClaimTypes.TenantId);
            if (tenantIdOrNull == null)
            {
                return null;
            }

            return Convert.ToInt32(tenantIdOrNull);
        }
    }
}