using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using StudioX.Auditing;
using StudioX.Configuration;
using StudioX.Localization;
using StudioX.Runtime.Session;
using StudioX.Timing;
using StudioX.Web.Configuration;
using StudioX.Web.Models;

namespace StudioX.Web.Mvc.Controllers.Localization
{
    public class StudioXLocalizationController : StudioXController
    {
        private readonly IStudioXWebLocalizationConfiguration _webLocalizationConfiguration;

        public StudioXLocalizationController(IStudioXWebLocalizationConfiguration webLocalizationConfiguration)
        {
            _webLocalizationConfiguration = webLocalizationConfiguration;
        }

        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new StudioXException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            Response.Cookies.Add(
                new HttpCookie(_webLocalizationConfiguration.CookieName, cultureName)
                {
                    Expires = Clock.Now.AddYears(2),
                    Path = Request.ApplicationPath
                }
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
                return Json(new AjaxResponse(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Request.Url != null && StudioXUrlHelper.IsLocalUrl(Request.Url, returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(Request.ApplicationPath);
        }
    }
}
