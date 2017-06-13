using System.Globalization;
using System.Text;
using StudioX.Extensions;
using StudioX.Web;
using StudioX.Web.Api.ProxyScripting.Generators;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.Angular
{
    internal class AngularActionScriptWriter
    {
        private readonly DynamicApiControllerInfo controllerInfo;
        private readonly DynamicApiActionInfo actionInfo;

        public AngularActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            this.controllerInfo = controllerInfo;
            actionInfo = methodInfo;
        }

        public virtual void WriteTo(StringBuilder script)
        {
            script.AppendLine("                this" + ProxyScriptingJsFuncHelper.WrapWithBracketsOrWithDotPrefix(actionInfo.ActionName.ToCamelCase()) + " = function (" + ActionScriptingHelper.GenerateJsMethodParameterList(actionInfo.Method, "httpParams") + ") {");
            script.AppendLine("                    return $http(angular.extend({");
            script.AppendLine("                        url: studiox.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(controllerInfo, actionInfo) + "',");
            script.AppendLine("                        method: '" + actionInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

            if (actionInfo.Verb == HttpVerb.Get)
            {
                script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(actionInfo));
            }
            else
            {
                script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(actionInfo) + ")");
            }

            script.AppendLine("                    }, httpParams));");
            script.AppendLine("                };");
            script.AppendLine("                ");
        }
    }
}