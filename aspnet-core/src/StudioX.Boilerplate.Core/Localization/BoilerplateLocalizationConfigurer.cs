using System.Reflection;
using StudioX.Configuration.Startup;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Reflection.Extensions;

namespace StudioX.Boilerplate.Localization
{
    public static class BoilerplateLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(BoilerplateConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(BoilerplateLocalizationConfigurer).GetAssembly(),
                        "StudioX.Boilerplate.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}