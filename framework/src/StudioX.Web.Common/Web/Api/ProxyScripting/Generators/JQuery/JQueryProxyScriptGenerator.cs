using System.Text;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Web.Api.Modeling;

namespace StudioX.Web.Api.ProxyScripting.Generators.JQuery
{
    public class JQueryProxyScriptGenerator : IProxyScriptGenerator, ITransientDependency
    {
        /// <summary>
        /// "jquery".
        /// </summary>
        public const string Name = "jquery";

        public string CreateScript(ApplicationApiDescriptionModel model)
        {
            var script = new StringBuilder();

            script.AppendLine("/* This file is automatically generated by StudioX framework to use MVC Controllers from javascript. */");
            script.AppendLine();
            script.AppendLine("var studiox = studiox || {};");
            script.AppendLine("studiox.services = studiox.services || {};");
            
            foreach (var module in model.Modules.Values)
            {
                script.AppendLine();
                AddModuleScript(script, module);
            }

            return script.ToString();
        }

        private static void AddModuleScript(StringBuilder script, ModuleApiDescriptionModel module)
        {
            script.AppendLine($"// module '{module.Name.ToCamelCase()}'");
            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine($"  studiox.services.{module.Name.ToCamelCase()} = studiox.services.{module.Name.ToCamelCase()} || {{}};");

            foreach (var controller in module.Controllers.Values)
            {
                script.AppendLine();
                AddControllerScript(script, module, controller);
            }

            script.AppendLine();
            script.AppendLine("})();");
        }

        private static void AddControllerScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller)
        {
            script.AppendLine($"  // controller '{controller.Name.ToCamelCase()}'");
            script.AppendLine("  (function(){");
            script.AppendLine();

            script.AppendLine($"    studiox.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()} = studiox.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()} || {{}};");

            foreach (var action in controller.Actions.Values)
            {
                script.AppendLine();
                AddActionScript(script, module, controller, action);
            }

            script.AppendLine();
            script.AppendLine("  })();");
        }

        private static void AddActionScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var parameterList = ProxyScriptingJsFuncHelper.GenerateJsFuncParameterList(action, "ajaxParams");

            script.AppendLine($"    // action '{action.Name.ToCamelCase()}'");
            script.AppendLine($"    studiox.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()}{ProxyScriptingJsFuncHelper.WrapWithBracketsOrWithDotPrefix(action.Name.ToCamelCase())} = function({parameterList}) {{");
            script.AppendLine("      return studiox.ajax($.extend(true, {");

            AddAjaxCallParameters(script, controller, action);

            script.AppendLine("      }, ajaxParams));;");
            script.AppendLine("    };");
        }

        private static void AddAjaxCallParameters(StringBuilder script, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var httpMethod = action.HttpMethod?.ToUpperInvariant() ?? "POST";

            script.AppendLine("        url: studiox.appPath + '" + ProxyScriptingHelper.GenerateUrlWithParameters(action) + "',");
            script.Append("        type: '" + httpMethod + "'");

            if (action.ReturnValue.Type == typeof(void))
            {
                script.AppendLine(",");
                script.Append("        dataType: null");
            }

            var headers = ProxyScriptingHelper.GenerateHeaders(action, 8);
            if (headers != null)
            {
                script.AppendLine(",");
                script.Append("        headers: " + headers);
            }

            var body = ProxyScriptingHelper.GenerateBody(action);
            if (!body.IsNullOrEmpty())
            {
                script.AppendLine(",");
                script.Append("        data: JSON.stringify(" + body + ")");
            }
            else
            {
                var formData = ProxyScriptingHelper.GenerateFormPostData(action, 8);
                if (!formData.IsNullOrEmpty())
                {
                    script.AppendLine(",");
                    script.Append("        data: " + formData);
                }
            }
            
            script.AppendLine();
        }
    }
}