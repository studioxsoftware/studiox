using System.Collections.Generic;

namespace StudioX.Web.Security.AntiForgery
{
    public class StudioXAntiForgeryWebConfiguration : IStudioXAntiForgeryWebConfiguration
    {
        public bool IsEnabled { get; set; }

        public HashSet<HttpVerb> IgnoredHttpVerbs { get; }

        public StudioXAntiForgeryWebConfiguration()
        {
            IsEnabled = true;
            IgnoredHttpVerbs = new HashSet<HttpVerb> { HttpVerb.Get, HttpVerb.Head, HttpVerb.Options, HttpVerb.Trace };
        }
    }
}