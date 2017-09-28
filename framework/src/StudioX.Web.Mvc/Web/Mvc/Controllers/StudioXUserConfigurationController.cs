using System.Threading.Tasks;
using System.Web.Mvc;
using StudioX.Web.Configuration;

namespace StudioX.Web.Mvc.Controllers
{
    public class StudioXUserConfigurationController : StudioXController
    {
        private readonly StudioXUserConfigurationBuilder _studioXUserConfigurationBuilder;

        public StudioXUserConfigurationController(StudioXUserConfigurationBuilder studioxUserConfigurationBuilder)
        {
            _studioXUserConfigurationBuilder = studioxUserConfigurationBuilder;
        }

        public async Task<JsonResult> GetAll()
        {
            var userConfig = await _studioXUserConfigurationBuilder.GetAll();
            return Json(userConfig, JsonRequestBehavior.AllowGet);
        }
    }
}