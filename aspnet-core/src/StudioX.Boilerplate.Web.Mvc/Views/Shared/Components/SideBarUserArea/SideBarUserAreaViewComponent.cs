using System.Threading.Tasks;
using StudioX.Configuration.Startup;
using StudioX.Boilerplate.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.SideBarUserArea
{
    public class SideBarUserAreaViewComponent : BoilerplateViewComponent
    {
        private readonly ISessionAppService sessionAppService;
        private readonly IMultiTenancyConfig multiTenancyConfig;

        public SideBarUserAreaViewComponent(ISessionAppService sessionAppService,
            IMultiTenancyConfig multiTenancyConfig)
        {
            this.sessionAppService = sessionAppService;
            this.multiTenancyConfig = multiTenancyConfig;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new SideBarUserAreaViewModel
            {
                LoginInformations = await sessionAppService.GetCurrentLoginInformations(),
                IsMultiTenancyEnabled = multiTenancyConfig.IsEnabled,
            };

            return View(model);
        }
    }
}
