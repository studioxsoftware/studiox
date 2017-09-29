using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Dependency;
using StudioX.Runtime.Session;

namespace StudioX.Web.Features
{
    public class FeaturesScriptManager : IFeaturesScriptManager, ITransientDependency
    {
        public IStudioXSession StudioXSession { get; set; }

        private readonly IFeatureManager featureManager;
        private readonly IFeatureChecker featureChecker;

        public FeaturesScriptManager(IFeatureManager featureManager, IFeatureChecker featureChecker)
        {
            this.featureManager = featureManager;
            this.featureChecker = featureChecker;

            StudioXSession = NullStudioXSession.Instance;
        }

        public async Task<string> GetScriptAsync()
        {
            var allFeatures = featureManager.GetAll().ToList();
            var currentValues = new Dictionary<string, string>();

            if (StudioXSession.TenantId.HasValue)
            {
                var currentTenantId = StudioXSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = await featureChecker.GetValueAsync(currentTenantId, feature.Name);
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = feature.DefaultValue;
                }
            }

            var script = new StringBuilder();

            script.AppendLine("(function() {");

            script.AppendLine();

            script.AppendLine("    studiox.features = studiox.features || {};");

            script.AppendLine();

            script.AppendLine("    studiox.features.allFeatures = {");

            for (var i = 0; i < allFeatures.Count; i++)
            {
                var feature = allFeatures[i];
                script.AppendLine("        '" + feature.Name.Replace("'", @"\'") + "': {");
                script.AppendLine("             value: '" + currentValues[feature.Name].Replace(@"\", @"\\").Replace("'", @"\'") + "'");
                script.Append("        }");

                if (i < allFeatures.Count - 1)
                {
                    script.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                }
            }

            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}