using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Localization.Dictionaries;

namespace StudioX.Localization
{
    /// <summary>
    /// Extends <see cref="ILocalizationDictionaryProvider"/> to add tenant and database based localization.
    /// </summary>
    public class MultiTenantLocalizationDictionaryProvider : ILocalizationDictionaryProvider
    {
        public ILocalizationDictionary DefaultDictionary
        {
            get { return GetDefaultDictionary(); }
        }

        public IDictionary<string, ILocalizationDictionary> Dictionaries => GetDictionaries();

        private readonly ConcurrentDictionary<string, ILocalizationDictionary> dictionaries;

        private string sourceName;

        private readonly ILocalizationDictionaryProvider internalProvider;

        private readonly IIocManager iocManager;
        private ILanguageManager languageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionaryProvider"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionaryProvider(ILocalizationDictionaryProvider internalProvider, IIocManager iocManager)
        {
            this.internalProvider = internalProvider;
            this.iocManager = iocManager;
            dictionaries = new ConcurrentDictionary<string, ILocalizationDictionary>();
        }

        public void Initialize(string sourceName)
        {
            this.sourceName = sourceName;
            languageManager = iocManager.Resolve<ILanguageManager>();
            internalProvider.Initialize(this.sourceName);
        }

        protected virtual IDictionary<string, ILocalizationDictionary> GetDictionaries()
        {
            var languages = languageManager.GetLanguages();

            foreach (var language in languages)
            {
                dictionaries.GetOrAdd(language.Name, s => CreateLocalizationDictionary(language));
            }

            return dictionaries;
        }

        protected virtual ILocalizationDictionary GetDefaultDictionary()
        {
            var languages = languageManager.GetLanguages();
            if (!languages.Any())
            {
                throw new StudioXException("No language defined!");
            }

            var defaultLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (defaultLanguage == null)
            {
                throw new StudioXException("Default language is not defined!");
            }

            return dictionaries.GetOrAdd(defaultLanguage.Name, s => CreateLocalizationDictionary(defaultLanguage));
        }

        protected virtual IMultiTenantLocalizationDictionary CreateLocalizationDictionary(LanguageInfo language)
        {
            var internalDictionary =
                internalProvider.Dictionaries.GetOrDefault(language.Name) ??
                new EmptyDictionary(CultureInfo.GetCultureInfo(language.Name));

            var dictionary =  iocManager.Resolve<IMultiTenantLocalizationDictionary>(new
            {
                sourceName = sourceName,
                internalDictionary = internalDictionary
            });

            return dictionary;
        }

        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            ILocalizationDictionary existingDictionary;
            if (!internalProvider.Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out existingDictionary))
            {
                internalProvider.Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            var localizedStrings = dictionary.GetAllStrings();
            foreach (var localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }
    }
}