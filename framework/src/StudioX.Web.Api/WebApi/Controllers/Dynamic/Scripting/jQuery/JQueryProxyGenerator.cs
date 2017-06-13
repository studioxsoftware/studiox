using System.Text;
using StudioX.Extensions;
using StudioX.Web.Api.ProxyScripting.Generators;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.jQuery
{
    internal class JQueryProxyGenerator : IScriptProxyGenerator
    {
        private readonly DynamicApiControllerInfo controllerInfo;
        private readonly bool defineAmdModule;

        public JQueryProxyGenerator(DynamicApiControllerInfo controllerInfo, bool defineAmdModule = true)
        {
            this.controllerInfo = controllerInfo;
            this.defineAmdModule = defineAmdModule;
        }

        public string Generate()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    var serviceNamespace = studiox.utils.createNamespace(studiox, 'services." + controllerInfo.ServiceName.Replace("/", ".") + "');");
            script.AppendLine();

            //generate all methods
            foreach (var methodInfo in controllerInfo.Actions.Values)
            {
                AppendMethod(script, controllerInfo, methodInfo);
                script.AppendLine();
            }

            //generate amd module definition
            if (defineAmdModule)
            {
                script.AppendLine("    if(typeof define === 'function' && define.amd){");
                script.AppendLine("        define(function (require, exports, module) {");
                script.AppendLine("            return {");

                var methodNo = 0;
                foreach (var methodInfo in controllerInfo.Actions.Values)
                {
                    script.AppendLine("                '" + methodInfo.ActionName.ToCamelCase() + "' : serviceNamespace" + ProxyScriptingJsFuncHelper.WrapWithBracketsOrWithDotPrefix(methodInfo.ActionName.ToCamelCase()) + ((methodNo++) < (controllerInfo.Actions.Count - 1) ? "," : ""));
                }

                script.AppendLine("            };");
                script.AppendLine("        });");
                script.AppendLine("    }");
            }

            script.AppendLine();
            script.AppendLine("})();");

            return script.ToString();
        }

        private static void AppendMethod(StringBuilder script, DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            var generator = new JQueryActionScriptGenerator(controllerInfo, methodInfo);
            script.AppendLine(generator.GenerateMethod());
        }
    }
}