using System.Threading.Tasks;
using StudioX.Web.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.Mvc.Controllers
{
    public class StudioXUserConfigurationController: StudioXController
    {
        private readonly StudioXUserConfigurationBuilder studioXUserConfigurationBuilder;

        public StudioXUserConfigurationController(StudioXUserConfigurationBuilder userConfigurationBuilder)
        {
            studioXUserConfigurationBuilder = userConfigurationBuilder;
        }

        public async Task<JsonResult> GetAll()
        {
            var userConfig = await studioXUserConfigurationBuilder.GetAll();
            return Json(userConfig);
        }
    }
}