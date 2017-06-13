using System.Threading.Tasks;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.TenantChange
{
    public class TenantChangeViewComponent : ViewComponent
    {
        private readonly ISessionAppService sessionAppService;

        public TenantChangeViewComponent(ISessionAppService sessionAppService)
        {
            this.sessionAppService = sessionAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loginInfo = await sessionAppService.GetCurrentLoginInformations();
            var model = loginInfo.MapTo<TenantChangeViewModel>();
            return View(model);
        }
    }
}
