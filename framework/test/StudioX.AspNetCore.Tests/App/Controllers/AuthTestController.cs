using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.AspNetCore.Mvc.Controllers;
using StudioX.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.App.Controllers
{
    public class AuthTestController : StudioXController
    {
        public ActionResult NonAuthorizedAction()
        {
            return Content("public content");
        }

        [Authorize]
        public ActionResult AuthorizedAction()
        {
            return Content("secret content");
        }

        [StudioXMvcAuthorize]
        public ActionResult StudioXMvcAuthorizedAction()
        {
            return Content("secret content");
        }

        [StudioXMvcAuthorize]
        public AjaxResponse StudioXMvcAuthorizedActionReturnsObject()
        {
            return new AjaxResponse("OK");
        }
    }
}
