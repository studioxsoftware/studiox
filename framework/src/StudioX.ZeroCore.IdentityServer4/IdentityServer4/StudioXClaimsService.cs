using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudioX.Runtime.Security;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace StudioX.IdentityServer4
{
    public class StudioXClaimsService : DefaultClaimsService
    {
        public StudioXClaimsService(IProfileService profile, ILogger<DefaultClaimsService> logger)
            : base(profile, logger)
        {
        }

        protected override IEnumerable<Claim> GetOptionalClaims(ClaimsPrincipal subject)
        {
            var tenantClaim = subject.FindFirst(StudioXClaimTypes.TenantId);
            if (tenantClaim == null)
            {
                return base.GetOptionalClaims(subject);
            }
            else
            {
                return base.GetOptionalClaims(subject).Union(new[] { tenantClaim });
            }
        }
    }
}
