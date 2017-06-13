using System.Linq;
using System.Text;
using StudioX.Dependency;
using StudioX.Extensions;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    public class TypeScriptDefinitionGenerator : ITransientDependency
    {
        private readonly DynamicApiControllerManager dynamicApiControllerManager;

        public TypeScriptDefinitionGenerator(DynamicApiControllerManager dynamicApiControllerManager)
        {
            this.dynamicApiControllerManager = dynamicApiControllerManager;
        }

        public string GetScript()
        {
            var dynamicControllers = dynamicApiControllerManager.GetAll();
            
            StringBuilder script = new StringBuilder();
            if (dynamicControllers == null || dynamicControllers.Count == 0)
                return "";
            //sorting the controllers and use this sorting for detecting the servicePrefix change
            //we create module per servicePrefix
            var sortedDynamicControllers = dynamicControllers.OrderBy(z => z.ServiceName);
            var servicePrefix = GetServicePrefix(sortedDynamicControllers.First().ServiceName);
            if (servicePrefix.IsNullOrEmpty())
                script.AppendLine("declare module studiox.services");//Create a new Module
            else
                script.AppendLine("declare module studiox.services." + servicePrefix);//Create a new Module
            script.AppendLine("{");
            var proxyGenerator = new TypeScriptDefinitionProxyGenerator();
            foreach (var dynamicController in sortedDynamicControllers)
            {
                if (servicePrefix != GetServicePrefix(dynamicController.ServiceName))
                {
                    //the service Prefix has been changed
                    servicePrefix = GetServicePrefix(dynamicController.ServiceName);
                    script.AppendLine("}");//Close the Previous Module
                    //Create new module for the new service prefix
                    if(servicePrefix.IsNullOrEmpty())
                        script.AppendLine("declare module studiox.services");//Create a new Module
                    else
                        script.AppendLine("declare module studiox.services." + servicePrefix);//Create a new Module
                    script.AppendLine("{");
                }
                script.AppendLine(proxyGenerator.Generate(dynamicController,servicePrefix));
                script.AppendLine();
            }
            script.AppendLine("}");
            return script.ToString();
        }

        private string GetServicePrefix(string serviceName)
        {
            if (serviceName.IndexOf('/') == -1)
                return  "";
            else
                return serviceName.Substring(0,serviceName.IndexOf('/'));
        }
    }
}
