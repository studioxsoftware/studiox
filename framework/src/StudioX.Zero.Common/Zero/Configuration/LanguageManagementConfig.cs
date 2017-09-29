using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.Localization.Dictionaries;
using Castle.Core.Logging;

namespace StudioX.Zero.Configuration
{
    internal class LanguageManagementConfig : ILanguageManagementConfig
    {
        public ILogger Logger { get; set; }

        private readonly IIocManager iocManager;
        private readonly IStudioXStartupConfiguration configuration;

        public LanguageManagementConfig(IIocManager iocManager, IStudioXStartupConfiguration configuration)
        {
            this.iocManager = iocManager;
            this.configuration = configuration;

            Logger = NullLogger.Instance;
        }

        public void EnableDbLocalization()
        {
            iocManager.Register<ILanguageProvider, ApplicationLanguageProvider>(DependencyLifeStyle.Transient);

            var sources = configuration
                .Localization
                .Sources
                .Where(s => s is IDictionaryBasedLocalizationSource)
                .Cast<IDictionaryBasedLocalizationSource>()
                .ToList();
            
            foreach (var source in sources)
            {
                configuration.Localization.Sources.Remove(source);
                configuration.Localization.Sources.Add(
                    new MultiTenantLocalizationSource(
                        source.Name,
                        new MultiTenantLocalizationDictionaryProvider(
                            source.DictionaryProvider,
                            iocManager
                            )
                        )
                    );

                Logger.DebugFormat("Converted {0} ({1}) to MultiTenantLocalizationSource", source.Name, source.GetType());
            }
        }
    }
}