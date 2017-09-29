using System;
using System.Globalization;
using System.Text;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.MultiTenancy;

namespace StudioX.Web.MultiTenancy
{
    public class MultiTenancyScriptManager : IMultiTenancyScriptManager, ITransientDependency
    {
        private readonly IMultiTenancyConfig multiTenancyConfig;

        public MultiTenancyScriptManager(IMultiTenancyConfig multiTenancyConfig)
        {
            this.multiTenancyConfig = multiTenancyConfig;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(studiox){");
            script.AppendLine();

            script.AppendLine("    studiox.multiTenancy = studiox.multiTenancy || {};");
            script.AppendLine("    studiox.multiTenancy.isEnabled = " + multiTenancyConfig.IsEnabled.ToString().ToLowerInvariant() + ";");

            script.AppendLine();
            script.Append("})(studiox);");

            return script.ToString();
        }
    }
}