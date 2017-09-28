using System.Reflection;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Localization.Sources;
using StudioX.Modules;
using StudioX.Zero.Ldap.Configuration;

namespace StudioX.Zero.Ldap
{
    /// <summary>
    /// This module extends module zero to add LDAP authentication.
    /// </summary>
    [DependsOn(typeof (StudioXZeroCommonModule))]
    public class StudioXZeroLdapModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXZeroLdapModuleConfig, StudioXZeroLdapModuleConfig>();

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    StudioXZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "StudioX.Zero.Ldap.Localization.Source")
                    )
                );

            Configuration.Settings.Providers.Add<LdapSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
