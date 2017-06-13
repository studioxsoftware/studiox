using System.Reflection;
using StudioX.AutoMapper;
using StudioX.Localization;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Json;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioXAspNetCoreDemo.Core
{
    [DependsOn(typeof(StudioXAutoMapperModule))]
    public class StudioXAspNetCoreDemoCoreModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe"));

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource("StudioXAspNetCoreDemoModule",
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(StudioXAspNetCoreDemoCoreModule).GetAssembly(),
                        "StudioXAspNetCoreDemo.Core.Localization.SourceFiles"
                    )
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXAspNetCoreDemoCoreModule).GetAssembly());
        }
    }
}