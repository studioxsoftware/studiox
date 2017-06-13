using System.Threading.Tasks;
using StudioX.Application.Navigation;
using StudioX.Runtime.Session;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.SideBarNav
{
    public class SideBarNavViewComponent : ViewComponent
    {
        private readonly IUserNavigationManager userNavigationManager;
        private readonly IStudioXSession session;

        public SideBarNavViewComponent(
            IUserNavigationManager userNavigationManager,
            IStudioXSession session)
        {
            this.userNavigationManager = userNavigationManager;
            this.session = session;
        }

        public async Task<IViewComponentResult> InvokeAsync(string activeMenu = "")
        {
            var model = new SideBarNavViewModel
            {
                MainMenu = await userNavigationManager.GetMenuAsync("MainMenu", session.ToUserIdentifier()),
                ActiveMenuItemName = activeMenu
            };

            return View(model);
        }
    }
}
