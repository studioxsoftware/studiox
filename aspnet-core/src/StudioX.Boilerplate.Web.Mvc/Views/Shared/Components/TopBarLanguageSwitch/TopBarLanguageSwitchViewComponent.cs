using System.Linq;
using StudioX.Localization;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.TopBarLanguageSwitch
{
    public class TopBarLanguageSwitchViewComponent : ViewComponent
    {
        private readonly ILanguageManager languageManager;

        public TopBarLanguageSwitchViewComponent(ILanguageManager languageManager)
        {
            this.languageManager = languageManager;
        }

        public IViewComponentResult Invoke()
        {
            var model = new TopBarLanguageSwitchViewModel
            {
                CurrentLanguage = languageManager.CurrentLanguage,
                Languages = languageManager.GetLanguages().Where(l => !l.IsDisabled).ToList()
            };

            return View(model);
        }
    }
}
