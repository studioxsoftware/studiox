using StudioX.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace StudioX.AspNetCore.Security.AntiForgery
{
    public class StudioXAspNetCoreAntiForgeryManager : IStudioXAntiForgeryManager
    {
        public IStudioXAntiForgeryConfiguration Configuration { get; }

        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudioXAspNetCoreAntiForgeryManager(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IStudioXAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken()
        {
            return _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;
        }
    }
}