using StudioX.Boilerplate.Controllers;
using Microsoft.AspNetCore.Antiforgery;

namespace StudioX.Boilerplate.Web.Host.Controllers
{
    public class AntiForgeryController : BoilerplateControllerBase
    {
        private readonly IAntiforgery antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public void GetToken()
        {
            antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}