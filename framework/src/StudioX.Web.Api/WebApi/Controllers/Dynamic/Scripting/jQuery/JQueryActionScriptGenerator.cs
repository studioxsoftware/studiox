using System.Text;
using StudioX.Extensions;
using StudioX.Web;
using StudioX.Web.Api.ProxyScripting.Generators;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.jQuery
{
    internal class JQueryActionScriptGenerator
    {
        private readonly DynamicApiControllerInfo controllerInfo;
        private readonly DynamicApiActionInfo actionInfo;

        private const string JsMethodTemplate =
@"    serviceNamespace{jsMethodName} = function({jsMethodParameterList}) {
        return studiox.ajax($.extend({
{ajaxCallParameters}
        }, ajaxParams));
    };";

        public JQueryActionScriptGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo actionInfo)
        {
            this.controllerInfo = controllerInfo;
            this.actionInfo = actionInfo;
        }

        public virtual string GenerateMethod()
        {
            var jsMethodName = actionInfo.ActionName.ToCamelCase();
            var jsMethodParameterList = ActionScriptingHelper.GenerateJsMethodParameterList(actionInfo.Method, "ajaxParams");

            var jsMethod = JsMethodTemplate
                .Replace("{jsMethodName}", ProxyScriptingJsFuncHelper.WrapWithBracketsOrWithDotPrefix(jsMethodName))
                .Replace("{jsMethodParameterList}", jsMethodParameterList)
                .Replace("{ajaxCallParameters}", GenerateAjaxCallParameters());

            return jsMethod;
        }

        protected string GenerateAjaxCallParameters()
        {
            var script = new StringBuilder();
            
            script.AppendLine("            url: studiox.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(controllerInfo, actionInfo) + "',");
            script.AppendLine("            type: '" + actionInfo.Verb.ToString().ToUpperInvariant() + "',");

            if (actionInfo.Verb == HttpVerb.Get)
            {
                script.Append("            data: " + ActionScriptingHelper.GenerateBody(actionInfo));
            }
            else
            {
                script.Append("            data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(actionInfo) + ")");                
            }
            
            return script.ToString();
        }
    }
}