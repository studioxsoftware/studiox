using System.Threading.Tasks;
using StudioX.Web.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.Mvc.Controllers
{
    public class StudioXUserConfigurationController: StudioXController
    {
        private readonly StudioXUserConfigurationBuilder _studioXUserConfigurationBuilder;

        public StudioXUserConfigurationController(StudioXUserConfigurationBuilder studioxUserConfigurationBuilder)
        {
            _studioXUserConfigurationBuilder = studioxUserConfigurationBuilder;
        }

        public async Task<JsonResult> GetAll()
        {
            var userConfig = await _studioXUserConfigurationBuilder.GetAll();
            return Json(userConfig);
        }
    }
}