using StudioX.AspNetCore.Mvc.Views;
using StudioX.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace StudioX.Boilerplate.Web.Views
{
    public abstract class BoilerplateRazorPage<TModel> : StudioXRazorPage<TModel>
    {
        [RazorInject]
        public IStudioXSession StudioXSession { get; set; }

        protected BoilerplateRazorPage()
        {
            LocalizationSourceName = BoilerplateConsts.LocalizationSourceName;
        }
    }
}
