using System.Collections.Generic;

namespace StudioX.Web.Security.AntiForgery
{
    /// <summary>
    /// Common configuration shared between ASP.NET MVC and ASP.NET Web API.
    /// </summary>
    public interface IStudioXAntiForgeryWebConfiguration
    {
        /// <summary>
        /// Used to enable/disable Anti Forgery token security mechanism of StudioX.
        /// Default: true (enabled).
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// A list of ignored HTTP verbs for Anti Forgery token validation.
        /// Default list: Get, Head, Options, Trace.
        /// </summary>
        HashSet<HttpVerb> IgnoredHttpVerbs { get; }
    }
}
