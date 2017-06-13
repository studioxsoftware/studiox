using System.Linq;
using System.Threading.Tasks;
using StudioX.Configuration;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Configuration.Ui;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.RightSideBar
{
    public class RightSideBarViewComponent : ViewComponent
    {
        private readonly ISettingManager settingManager;

        public RightSideBarViewComponent(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var themeName = await settingManager.GetSettingValueAsync(AppSettingNames.UiTheme);

            var viewModel = new RightSideBarViewModel
            {
                CurrentTheme = UiThemes.All.FirstOrDefault(t => t.CssClass == themeName)
            };

            return View(viewModel);
        }
    }
}
