using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.Auditing;
using StudioX.Configuration;
using StudioX.Localization;
using StudioX.Runtime.Session;
using StudioX.Timing;
using StudioX.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.Mvc.Controllers
{
    public class StudioXLocalizationController : StudioXController
    {
        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new StudioXException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName, cultureName));

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                new CookieOptions {Expires = Clock.Now.AddYears(2)}
            );

            if (StudioXSession.UserId.HasValue)
            {
                SettingManager.ChangeSettingForUser(
                    StudioXSession.ToUserIdentifier(),
                    LocalizationSettingNames.DefaultLanguage,
                    cultureName
                );
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new AjaxResponse());
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && StudioXUrlHelper.IsLocalUrl(Request, returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/"); //TODO: Go to app root
        }
    }
}
