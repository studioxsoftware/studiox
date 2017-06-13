using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StudioX.Auditing;
using StudioX.Extensions;
using StudioX.Localization;
using StudioX.Web.Authorization;
using StudioX.Web.Features;
using StudioX.Web.Localization;
using StudioX.Web.MultiTenancy;
using StudioX.Web.Navigation;
using StudioX.Web.Sessions;
using StudioX.Web.Settings;
using StudioX.Web.Timing;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.Mvc.Controllers
{
    /// <summary>
    /// This controller is used to create client side scripts
    /// to work with StudioX.
    /// </summary>
    public class StudioXScriptsController : StudioXController
    {
        private readonly IMultiTenancyScriptManager multiTenancyScriptManager;
        private readonly ISettingScriptManager settingScriptManager;
        private readonly INavigationScriptManager navigationScriptManager;
        private readonly ILocalizationScriptManager localizationScriptManager;
        private readonly IAuthorizationScriptManager authorizationScriptManager;
        private readonly IFeaturesScriptManager featuresScriptManager;
        private readonly ISessionScriptManager sessionScriptManager;
        private readonly ITimingScriptManager timingScriptManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StudioXScriptsController(
            IMultiTenancyScriptManager multiTenancyScriptManager,
            ISettingScriptManager settingScriptManager,
            INavigationScriptManager navigationScriptManager,
            ILocalizationScriptManager localizationScriptManager,
            IAuthorizationScriptManager authorizationScriptManager,
            IFeaturesScriptManager featuresScriptManager,
            ISessionScriptManager sessionScriptManager, 
            ITimingScriptManager timingScriptManager)
        {
            this.multiTenancyScriptManager = multiTenancyScriptManager;
            this.settingScriptManager = settingScriptManager;
            this.navigationScriptManager = navigationScriptManager;
            this.localizationScriptManager = localizationScriptManager;
            this.authorizationScriptManager = authorizationScriptManager;
            this.featuresScriptManager = featuresScriptManager;
            this.sessionScriptManager = sessionScriptManager;
            this.timingScriptManager = timingScriptManager;
        }

        /// <summary>
        /// Gets all needed scripts.
        /// </summary>
        [DisableAuditing]
        public async Task<ActionResult> GetScripts(string culture = "")
        {
            if (!culture.IsNullOrEmpty())
            {
                CultureInfo.CurrentCulture = CultureInfoHelper.Get(culture);
                CultureInfo.CurrentUICulture = CultureInfoHelper.Get(culture);
            }

            var sb = new StringBuilder();

            sb.AppendLine(multiTenancyScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(sessionScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(localizationScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(await featuresScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await authorizationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await navigationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await settingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await timingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(GetTriggerScript());
            
            return Content(sb.ToString(), "application/x-javascript", Encoding.UTF8);
        }

        private static string GetTriggerScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    studiox.event.trigger('studiox.dynamicScriptsInitialized');");
            script.Append("})();");

            return script.ToString();
        }
    }
}
