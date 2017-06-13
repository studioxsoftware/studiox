using StudioX.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StudioXAspNetCoreDemo.PlugIn.Controllers
{
    public class BlogController : StudioXController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
