using StudioX.AspNetCore.Mvc.Controllers;

namespace StudioXAspNetCoreDemo.Controllers
{
    public class DemoControllerBase : StudioXController
    {
        public DemoControllerBase()
        {
            LocalizationSourceName = "StudioXAspNetCoreDemoModule";
        }
    }
}
