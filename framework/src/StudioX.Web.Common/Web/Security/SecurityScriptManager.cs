using System.Text;
using StudioX.Dependency;
using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly IStudioXAntiForgeryConfiguration studioXAntiForgeryConfiguration;

        public SecurityScriptManager(IStudioXAntiForgeryConfiguration antiForgeryConfiguration)
        {
            studioXAntiForgeryConfiguration = antiForgeryConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    studiox.security.antiForgery.tokenCookieName = '" + studioXAntiForgeryConfiguration.TokenCookieName + "';");
            script.AppendLine("    studiox.security.antiForgery.tokenHeaderName = '" + studioXAntiForgeryConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
