using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using StudioX.Extensions;
using StudioX.WebApi.Extensions;

namespace StudioX.Web.Security.AntiForgery
{
    public static class StudioXAntiForgeryManagerWebApiExtensions
    {
        public static void SetCookie(this IStudioXAntiForgeryManager manager, HttpResponseHeaders headers)
        {
            headers.SetCookie(new Cookie(manager.Configuration.TokenCookieName, manager.GenerateToken()));
        }

        public static bool IsValid(this IStudioXAntiForgeryManager manager, HttpRequestHeaders headers)
        {
            var cookieTokenValue = GetCookieValue(manager, headers);
            if (cookieTokenValue.IsNullOrEmpty())
            {
                return true;
            }

            var headerTokenValue = GetHeaderValue(manager, headers);
            if (headerTokenValue.IsNullOrEmpty())
            {
                return false;
            }

            return manager.As<IStudioXAntiForgeryValidator>().IsValid(cookieTokenValue, headerTokenValue);
        }

        private static string GetCookieValue(IStudioXAntiForgeryManager manager, HttpRequestHeaders headers)
        {
            var cookie = headers.GetCookies(manager.Configuration.TokenCookieName).LastOrDefault();
            if (cookie == null)
            {
                return null;
            }

            return cookie[manager.Configuration.TokenCookieName].Value;
        }

        private static string GetHeaderValue(IStudioXAntiForgeryManager manager, HttpRequestHeaders headers)
        {
            IEnumerable<string> headerValues;
            if (!headers.TryGetValues(manager.Configuration.TokenHeaderName, out headerValues))
            {
                return null;
            }

            var headersArray = headerValues.ToArray();
            if (!headersArray.Any())
            {
                return null;
            }
            
            return headersArray.Last().Split(", ").Last();
        }
    }
}
