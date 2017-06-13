using System.Reflection;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Localization.Sources;
using StudioX.Modules;

namespace StudioX.Zero
{
    [DependsOn(typeof(StudioXZeroCommonModule))]
    public class StudioXZeroCoreModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    StudioXZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(), "StudioX.Zero.Localization.SourceExt"
                    )
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
