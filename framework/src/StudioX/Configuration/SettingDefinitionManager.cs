using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Configuration.Startup;
using StudioX.Dependency;

namespace StudioX.Configuration
{
    /// <summary>
    ///     Implements <see cref="ISettingDefinitionManager" />.
    /// </summary>
    internal class SettingDefinitionManager : ISettingDefinitionManager, ISingletonDependency
    {
        private readonly IIocManager iocManager;
        private readonly ISettingsConfiguration settingsConfiguration;
        private readonly IDictionary<string, SettingDefinition> settings;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public SettingDefinitionManager(IIocManager iocManager, ISettingsConfiguration settingsConfiguration)
        {
            this.iocManager = iocManager;
            this.settingsConfiguration = settingsConfiguration;
            settings = new Dictionary<string, SettingDefinition>();
        }

        public void Initialize()
        {
            var context = new SettingDefinitionProviderContext(this);

            foreach (var providerType in settingsConfiguration.Providers)
            {
                using (var provider = CreateProvider(providerType))
                {
                    foreach (var settings in provider.Object.GetSettingDefinitions(context))
                    {
                        this.settings[settings.Name] = settings;
                    }
                }
            }
        }

        public SettingDefinition GetSettingDefinition(string name)
        {
            SettingDefinition settingDefinition;
            if (!settings.TryGetValue(name, out settingDefinition))
            {
                throw new StudioXException("There is no setting defined with name: " + name);
            }

            return settingDefinition;
        }

        public IReadOnlyList<SettingDefinition> GetAllSettingDefinitions()
        {
            return settings.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<SettingProvider> CreateProvider(Type providerType)
        {
            return iocManager.ResolveAsDisposable<SettingProvider>(providerType);
        }
    }
}