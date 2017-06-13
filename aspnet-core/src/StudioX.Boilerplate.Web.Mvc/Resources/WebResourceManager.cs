using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Collections.Extensions;
using StudioX.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;

namespace StudioX.Boilerplate.Web.Resources
{
    public class WebResourceManager : IWebResourceManager
    {
        private readonly IHostingEnvironment environment;
        private readonly List<string> scriptUrls;

        public WebResourceManager(IHostingEnvironment environment)
        {
            this.environment = environment;
            scriptUrls = new List<string>();
        }

        public void AddScript(string url, bool addMinifiedOnProd = true)
        {
            scriptUrls.AddIfNotContains(NormalizeUrl(url, "js"));
        }

        public IReadOnlyList<string> GetScripts()
        {
            return scriptUrls.ToImmutableList();
        }

        public HelperResult RenderScripts()
        {
            return new HelperResult(async writer =>
            {
                foreach (var scriptUrl in scriptUrls)
                {
                    await writer.WriteAsync($"<script src=\"{scriptUrl}\" asp-append-version=\"true\"></script>");
                }
            });
        }

        private string NormalizeUrl(string url, string ext)
        {
            if (environment.IsDevelopment())
            {
                return url;
            }

            if (url.EndsWith(".min." + ext))
            {
                return url;
            }

            return url.Left(url.Length - ext.Length) + "min." + ext;
        }
    }
}