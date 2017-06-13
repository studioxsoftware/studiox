using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace StudioX.Web.Security.AntiForgery
{
    public static class StudioXAntiForgeryManagerAspNetCoreExtensions
    {
        public static void SetCookie(this IStudioXAntiForgeryManager manager, HttpContext context, IIdentity identity = null)
        {
            if (identity != null)
            {
                context.User = new ClaimsPrincipal(identity);
            }

            context.Response.Cookies.Append(manager.Configuration.TokenCookieName, manager.GenerateToken());
        }
    }
}
