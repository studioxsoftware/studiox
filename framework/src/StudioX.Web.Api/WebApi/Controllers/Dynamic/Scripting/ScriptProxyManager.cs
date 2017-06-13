using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.WebApi.Controllers.Dynamic.Scripting.Angular;
using StudioX.WebApi.Controllers.Dynamic.Scripting.jQuery;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting
{
    //TODO@Long: This class can be optimized.
    public class ScriptProxyManager : ISingletonDependency
    {
        private readonly IDictionary<string, ScriptInfo> CachedScripts;
        private readonly DynamicApiControllerManager dynamicApiControllerManager;

        public ScriptProxyManager(DynamicApiControllerManager dynamicApiControllerManager)
        {
            this.dynamicApiControllerManager = dynamicApiControllerManager;
            CachedScripts = new Dictionary<string, ScriptInfo>();
        }

        public string GetScript(string name, ProxyScriptType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name is null or empty!", nameof(name));
            }

            var cacheKey = type + "_" + name;

            lock (CachedScripts)
            {
                var cachedScript = CachedScripts.GetOrDefault(cacheKey);
                if (cachedScript == null)
                {
                    var dynamicController = dynamicApiControllerManager
                        .GetAll()
                        .FirstOrDefault(ci => ci.ServiceName == name && ci.IsProxyScriptingEnabled);

                    if (dynamicController == null)
                    {
                        throw new HttpException((int)HttpStatusCode.NotFound, "There is no such a service: " + cacheKey);
                    }

                    var script = CreateProxyGenerator(type, dynamicController, true).Generate();
                    CachedScripts[cacheKey] = cachedScript = new ScriptInfo(script);
                }

                return cachedScript.Script;
            }
        }

        public string GetAllScript(ProxyScriptType type)
        {
            lock (CachedScripts)
            {
                var cacheKey = type + "_all";
                if (!CachedScripts.ContainsKey(cacheKey))
                {
                    var script = new StringBuilder();

                    var dynamicControllers = dynamicApiControllerManager.GetAll().Where(ci => ci.IsProxyScriptingEnabled);
                    foreach (var dynamicController in dynamicControllers)
                    {
                        var proxyGenerator = CreateProxyGenerator(type, dynamicController, false);
                        script.AppendLine(proxyGenerator.Generate());
                        script.AppendLine();
                    }

                    CachedScripts[cacheKey] = new ScriptInfo(script.ToString());
                }

                return CachedScripts[cacheKey].Script;
            }
        }

        private static IScriptProxyGenerator CreateProxyGenerator(ProxyScriptType type, DynamicApiControllerInfo controllerInfo, bool amdModule)
        {
            switch (type)
            {
                case ProxyScriptType.JQuery:
                    return new JQueryProxyGenerator(controllerInfo, amdModule);
                case ProxyScriptType.Angular:
                    return new AngularProxyGenerator(controllerInfo);
                default:
                    throw new StudioXException("Unknown ProxyScriptType: " + type);
            }
        }

        private class ScriptInfo
        {
            public string Script { get; private set; }

            public ScriptInfo(string script)
            {
                Script = script;
            }
        }
    }
}
