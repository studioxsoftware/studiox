using StudioX.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace StudioX.AspNetCore.Security.AntiForgery
{
    public class StudioXAspNetCoreAntiForgeryManager : IStudioXAntiForgeryManager
    {
        public IStudioXAntiForgeryConfiguration Configuration { get; }

        private readonly IAntiforgery antiforgery;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StudioXAspNetCoreAntiForgeryManager(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IStudioXAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            this.antiforgery = antiforgery;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken()
        {
            return antiforgery.GetAndStoreTokens(httpContextAccessor.HttpContext).RequestToken;
        }
    }
}