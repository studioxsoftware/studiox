using System.Linq;
using System.Threading.Tasks;
using StudioX.Localization;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.AccountLanguages
{
    public class AccountLanguagesViewComponent : BoilerplateViewComponent
    {
        private readonly ILanguageManager languageManager;

        public AccountLanguagesViewComponent(ILanguageManager languageManager)
        {
            this.languageManager = languageManager;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var model = new LanguageSelectionViewModel
            {
                CurrentLanguage = languageManager.CurrentLanguage,
                Languages = languageManager.GetLanguages().Where(l => !l.IsDisabled).ToList(),
                CurrentUrl = Request.Path
            };

            return Task.FromResult(View(model) as IViewComponentResult);
        }
    }
}
