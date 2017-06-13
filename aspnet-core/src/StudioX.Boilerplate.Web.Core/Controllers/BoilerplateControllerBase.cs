using StudioX.AspNetCore.Mvc.Controllers;
using StudioX.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace StudioX.Boilerplate.Controllers
{
    public abstract class BoilerplateControllerBase: StudioXController
    {
        protected BoilerplateControllerBase()
        {
            LocalizationSourceName = BoilerplateConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}