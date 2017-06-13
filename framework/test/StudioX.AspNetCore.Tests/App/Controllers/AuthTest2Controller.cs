using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.App.Controllers
{
    [StudioXMvcAuthorize]
    public class AuthTest2Controller : StudioXController
    {
        [AllowAnonymous]
        public ActionResult NonAuthorizedAction()
        {
            return Content("public content 2");
        }
        
        public ActionResult AuthorizedAction()
        {
            return Content("secret content 2");
        }
    }
}