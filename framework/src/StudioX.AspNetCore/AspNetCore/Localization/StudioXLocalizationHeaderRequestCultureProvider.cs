using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Internal;

namespace StudioX.AspNetCore.Localization
{
    public class StudioXLocalizationHeaderRequestCultureProvider : RequestCultureProvider
    {
        private static readonly char[] Separator = { '|' };

        private static readonly string culturePrefix = "c=";
        private static readonly string uiCulturePrefix = "uic=";

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var localizationHeader = httpContext.Request.Headers[CookieRequestCultureProvider.DefaultCookieName];

            if (localizationHeader.Count == 0)
            {
                return TaskCache<ProviderCultureResult>.DefaultCompletedTask;
            }

            return Task.FromResult(ParseHeaderValue(localizationHeader));
        }

        /// <summary>
        /// Parses a <see cref="RequestCulture"/> from the specified cookie value.
        /// Returns <c>null</c> if parsing fails.
        /// </summary>
        /// <param name="value">The cookie value to parse.</param>
        /// <returns>The <see cref="RequestCulture"/> or <c>null</c> if parsing fails.</returns>
        public static ProviderCultureResult ParseHeaderValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (!value.Contains("|") && !value.Contains("="))
            {
                return new ProviderCultureResult(value, value);
            }

            var parts = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return null;
            }

            var potentialCultureName = parts[0];
            var potentialUiCultureName = parts[1];

            if (!potentialCultureName.StartsWith(culturePrefix) || !potentialUiCultureName.StartsWith(uiCulturePrefix))
            {
                return null;
            }

            var cultureName = potentialCultureName.Substring(culturePrefix.Length);
            var uiCultureName = potentialUiCultureName.Substring(uiCulturePrefix.Length);

            return new ProviderCultureResult(cultureName, uiCultureName);
        }
    }
}
