using System.Threading.Tasks;
using System.Web.Mvc;
using StudioX.Web.Configuration;

namespace StudioX.Web.Mvc.Controllers
{
    public class StudioXUserConfigurationController : StudioXController
    {
        private readonly StudioXUserConfigurationBuilder studioXUserConfigurationBuilder;

        public StudioXUserConfigurationController(StudioXUserConfigurationBuilder userConfigurationBuilder)
        {
            studioXUserConfigurationBuilder = userConfigurationBuilder;
        }

        public async Task<JsonResult> GetAll()
        {
            var userConfig = await studioXUserConfigurationBuilder.GetAll();
            return Json(userConfig, JsonRequestBehavior.AllowGet);
        }
    }
}