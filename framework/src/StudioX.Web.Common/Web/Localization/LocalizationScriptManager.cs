using System.Globalization;
using System.Linq;
using System.Text;
using StudioX.Dependency;
using StudioX.Json;
using StudioX.Localization;

namespace StudioX.Web.Localization
{
    internal class LocalizationScriptManager : ILocalizationScriptManager, ISingletonDependency
    {
        private readonly ILocalizationManager localizationManager;
        private readonly ILanguageManager languageManager;

        public LocalizationScriptManager(
            ILocalizationManager localizationManager,
            ILanguageManager languageManager)
        {
            this.localizationManager = localizationManager;
            this.languageManager = languageManager;
        }

        /// <inheritdoc/>
        public string GetScript()
        {
            return GetScript(CultureInfo.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetScript(CultureInfo cultureInfo)
        {
            //NOTE: Disabled caching since it's not true (localization script is changed per user, per tenant, per culture...)
            return BuildAll(cultureInfo);
            //return cacheManager.GetCache(StudioXCacheNames.LocalizationScripts).Get(cultureInfo.Name, () => BuildAll(cultureInfo));
        }

        private string BuildAll(CultureInfo cultureInfo)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    studiox.localization = studiox.localization || {};");
            script.AppendLine();
            script.AppendLine("    studiox.localization.currentCulture = {");
            script.AppendLine("        name: '" + cultureInfo.Name + "',");
            script.AppendLine("        displayName: '" + cultureInfo.DisplayName + "'");
            script.AppendLine("    };");
            script.AppendLine();
            script.Append("    studiox.localization.languages = [");

            var languages = languageManager.GetLanguages();
            for (var i = 0; i < languages.Count; i++)
            {
                var language = languages[i];

                script.AppendLine("{");
                script.AppendLine("        name: '" + language.Name + "',");
                script.AppendLine("        displayName: '" + language.DisplayName + "',");
                script.AppendLine("        icon: '" + language.Icon + "',");
                script.AppendLine("        isDisabled: '" + language.IsDisabled.ToString().ToLowerInvariant() + "',");
                script.AppendLine("        isDefault: " + language.IsDefault.ToString().ToLowerInvariant());
                script.Append("    }");

                if (i < languages.Count - 1)
                {
                    script.Append(" , ");
                }
            }

            script.AppendLine("];");
            script.AppendLine();

            if (languages.Count > 0)
            {
                var currentLanguage = languageManager.CurrentLanguage;
                script.AppendLine("    studiox.localization.currentLanguage = {");
                script.AppendLine("        name: '" + currentLanguage.Name + "',");
                script.AppendLine("        displayName: '" + currentLanguage.DisplayName + "',");
                script.AppendLine("        icon: '" + currentLanguage.Icon + "',");
                script.AppendLine("        isDisabled: '" + currentLanguage.IsDisabled.ToString().ToLowerInvariant() + "',");
                script.AppendLine("        isDefault: " + currentLanguage.IsDefault.ToString().ToLowerInvariant());
                script.AppendLine("    };");
            }

            var sources = localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();

            script.AppendLine();
            script.AppendLine("    studiox.localization.sources = [");

            for (int i = 0; i < sources.Length; i++)
            {
                var source = sources[i];
                script.AppendLine("        {");
                script.AppendLine("            name: '" + source.Name + "',");
                script.AppendLine("            type: '" + source.GetType().Name + "'");
                script.AppendLine("        }" + (i < (sources.Length - 1) ? "," : ""));
            }

            script.AppendLine("    ];");

            script.AppendLine();
            script.AppendLine("    studiox.localization.values = studiox.localization.values || {};");
            script.AppendLine();

            foreach (var source in sources)
            {
                script.Append("    studiox.localization.values['" + source.Name + "'] = ");

                var stringValues = source.GetAllStrings(cultureInfo).OrderBy(s => s.Name).ToList();
                var stringJson = stringValues
                    .ToDictionary(s => s.Name, s => s.Value)
                    .ToJsonString(indented: true);
                script.Append(stringJson);

                script.AppendLine(";");
                script.AppendLine();
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}
