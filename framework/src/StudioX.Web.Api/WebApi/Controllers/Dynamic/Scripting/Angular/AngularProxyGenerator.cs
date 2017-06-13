using System.Text;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.Angular
{
    internal class AngularProxyGenerator : IScriptProxyGenerator
    {
        private readonly DynamicApiControllerInfo controllerInfo;

        public AngularProxyGenerator(DynamicApiControllerInfo controllerInfo)
        {
            this.controllerInfo = controllerInfo;
        }

        public string Generate()
        {
            var script = new StringBuilder();

            script.AppendLine("(function (studiox, angular) {");
            script.AppendLine("");
            script.AppendLine("    if (!angular) {");
            script.AppendLine("        return;");
            script.AppendLine("    }");
            script.AppendLine("    ");
            script.AppendLine("    var studioxModule = angular.module('studiox');");
            script.AppendLine("    ");
            script.AppendLine("    studioxModule.factory('studiox.services." + controllerInfo.ServiceName.Replace("/", ".") + "', [");
            script.AppendLine("        '$http', function ($http) {");
            script.AppendLine("            return new function () {");

            foreach (var methodInfo in controllerInfo.Actions.Values)
            {
                var actionWriter = new AngularActionScriptWriter(controllerInfo, methodInfo);
                actionWriter.WriteTo(script);
            }

            script.AppendLine("            };");
            script.AppendLine("        }");
            script.AppendLine("    ]);");
            script.AppendLine();

            script.AppendLine();
            script.AppendLine("})((studiox || (studiox = {})), (angular || undefined));");

            return script.ToString();
        }
    }
}