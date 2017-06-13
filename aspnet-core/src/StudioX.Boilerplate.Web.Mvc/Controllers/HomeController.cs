using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.Boilerplate.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Controllers
{
    [StudioXMvcAuthorize]
    public class HomeController : BoilerplateControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}