using StudioX.AspNetCore.Mvc.ViewComponents;

namespace StudioX.Boilerplate.Web.Views
{
    public abstract class BoilerplateViewComponent : StudioXViewComponent
    {
        protected BoilerplateViewComponent()
        {
            LocalizationSourceName = BoilerplateConsts.LocalizationSourceName;
        }
    }
}
