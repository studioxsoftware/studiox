using StudioX.Boilerplate.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Controllers
{
    public class AboutController : BoilerplateControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}