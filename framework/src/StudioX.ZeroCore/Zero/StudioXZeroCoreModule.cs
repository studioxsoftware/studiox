using StudioX.Localization.Dictionaries.Xml;
using StudioX.Localization.Sources;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

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
                        typeof(StudioXZeroCoreModule).GetAssembly(), "StudioX.Zero.Localization.SourceExt"
                    )
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroCoreModule).GetAssembly());
        }
    }
}
