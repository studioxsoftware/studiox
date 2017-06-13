using System.Security.Claims;
using StudioX.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace StudioX.AspNetCore.Runtime.Session
{
    public class AspNetCorePrincipalAccessor : DefaultPrincipalAccessor
    {
        public override ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User ?? base.Principal;

        private readonly IHttpContextAccessor httpContextAccessor;

        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}
