using System.Text;
using StudioX.Dependency;
using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly IStudioXAntiForgeryConfiguration _studioXAntiForgeryConfiguration;

        public SecurityScriptManager(IStudioXAntiForgeryConfiguration studioxAntiForgeryConfiguration)
        {
            _studioXAntiForgeryConfiguration = studioxAntiForgeryConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    studiox.security.antiForgery.tokenCookieName = '" + _studioXAntiForgeryConfiguration.TokenCookieName + "';");
            script.AppendLine("    studiox.security.antiForgery.tokenHeaderName = '" + _studioXAntiForgeryConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
