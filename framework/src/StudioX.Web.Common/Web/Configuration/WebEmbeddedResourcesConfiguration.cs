using System.Collections.Generic;

namespace StudioX.Web.Configuration
{
    internal class WebEmbeddedResourcesConfiguration : IWebEmbeddedResourcesConfiguration
    {
        public HashSet<string> IgnoredFileExtensions { get; }

        public WebEmbeddedResourcesConfiguration()
        {
            IgnoredFileExtensions = new HashSet<string>
            {
                "cshtml",
                "config"
            };
        }
    }
}