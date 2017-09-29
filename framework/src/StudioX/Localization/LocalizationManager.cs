using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Sources;
using Castle.Core.Logging;

namespace StudioX.Localization
{
    internal class LocalizationManager : ILocalizationManager
    {
        public ILogger Logger { get; set; }

        private readonly ILanguageManager languageManager;
        private readonly ILocalizationConfiguration configuration;
        private readonly IIocResolver iocResolver;
        private readonly IDictionary<string, ILocalizationSource> sources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationManager(
            ILanguageManager languageManager,
            ILocalizationConfiguration configuration, 
            IIocResolver iocResolver)
        {
            Logger = NullLogger.Instance;
            this.languageManager = languageManager;
            this.configuration = configuration;
            this.iocResolver = iocResolver;
            sources = new Dictionary<string, ILocalizationSource>();
        }

        public void Initialize()
        {
            InitializeSources();
        }

        private void InitializeSources()
        {
            if (!configuration.IsEnabled)
            {
                Logger.Debug("Localization disabled.");
                return;
            }

            Logger.Debug(string.Format("Initializing {0} localization sources.", configuration.Sources.Count));
            foreach (var source in configuration.Sources)
            {
                if (sources.ContainsKey(source.Name))
                {
                    throw new StudioXException("There are more than one source with name: " + source.Name + "! Source name must be unique!");
                }

                sources[source.Name] = source;
                source.Initialize(configuration, iocResolver);

                //Extending dictionaries
                if (source is IDictionaryBasedLocalizationSource)
                {
                    var dictionaryBasedSource = source as IDictionaryBasedLocalizationSource;
                    var extensions = configuration.Sources.Extensions.Where(e => e.SourceName == source.Name).ToList();
                    foreach (var extension in extensions)
                    {
                        extension.DictionaryProvider.Initialize(source.Name);
                        foreach (var extensionDictionary in extension.DictionaryProvider.Dictionaries.Values)
                        {
                            dictionaryBasedSource.Extend(extensionDictionary);
                        }
                    }
                }

                Logger.Debug("Initialized localization source: " + source.Name);
            }
        }

        /// <summary>
        /// Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        public ILocalizationSource GetSource(string name)
        {
            if (!configuration.IsEnabled)
            {
                return NullLocalizationSource.Instance;
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            ILocalizationSource source;
            if (!sources.TryGetValue(name, out source))
            {
                throw new StudioXException("Can not find a source with name: " + name);
            }

            return source;
        }

        /// <summary>
        /// Gets all registered localization sources.
        /// </summary>
        /// <returns>List of sources</returns>
        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return sources.Values.ToImmutableList();
        }
    }
}