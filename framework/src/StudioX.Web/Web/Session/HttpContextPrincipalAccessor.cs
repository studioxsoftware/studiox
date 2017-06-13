using System.Security.Claims;
using System.Web;
using StudioX.Runtime.Session;

namespace StudioX.Web.Session
{
    public class HttpContextPrincipalAccessor : DefaultPrincipalAccessor
    {
        public override ClaimsPrincipal Principal => HttpContext.Current?.User as ClaimsPrincipal ?? base.Principal;
    }
}
