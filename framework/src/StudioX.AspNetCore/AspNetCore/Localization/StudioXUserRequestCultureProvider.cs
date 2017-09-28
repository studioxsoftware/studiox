using System.Linq;
using System.Threading.Tasks;
using StudioX.Configuration;
using StudioX.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using StudioX.Localization;
using StudioX.Extensions;
using JetBrains.Annotations;

namespace StudioX.AspNetCore.Localization
{
    public class StudioXUserRequestCultureProvider : RequestCultureProvider
    {
        public CookieRequestCultureProvider CookieProvider { get; set; }
        public StudioXLocalizationHeaderRequestCultureProvider HeaderProvider { get; set; }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var studioxSession = httpContext.RequestServices.GetRequiredService<IStudioXSession>();
            if (studioxSession.UserId == null)
            {
                return null;
            }

            var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

            var culture = await settingManager.GetSettingValueForUserAsync(
                LocalizationSettingNames.DefaultLanguage,
                studioxSession.TenantId,
                studioxSession.UserId.Value,
                fallbackToDefault: false
            );

            if (!culture.IsNullOrEmpty())
            {
                return new ProviderCultureResult(culture, culture);
            }

            var result = await GetResultOrNull(httpContext, CookieProvider) ??
                         await GetResultOrNull(httpContext, HeaderProvider);

            if (result == null || !result.Cultures.Any())
            {
                return null;
            }

            //Try to set user's language setting from cookie if available.
            await settingManager.ChangeSettingForUserAsync(
                studioxSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                result.Cultures.First().Value
            );

            return result;
        }

        protected virtual async Task<ProviderCultureResult> GetResultOrNull([NotNull] HttpContext httpContext, [CanBeNull] IRequestCultureProvider provider)
        {
            if (provider == null)
            {
                return null;
            }

            return await provider.DetermineProviderCultureResult(httpContext);
        }
    }
}
