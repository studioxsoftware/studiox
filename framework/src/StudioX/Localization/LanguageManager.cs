using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using StudioX.Dependency;

namespace StudioX.Localization
{
    public class LanguageManager : ILanguageManager, ITransientDependency
    {
        public LanguageInfo CurrentLanguage { get { return GetCurrentLanguage(); } }

        private readonly ILanguageProvider languageProvider;

        public LanguageManager(ILanguageProvider languageProvider)
        {
            this.languageProvider = languageProvider;
        }

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return languageProvider.GetLanguages();
        }

        private LanguageInfo GetCurrentLanguage()
        {
            var languages = languageProvider.GetLanguages();
            if (languages.Count <= 0)
            {
                throw new StudioXException("No language defined in this application.");
            }

            var currentCultureName = CultureInfo.CurrentUICulture.Name;

            //Try to find exact match
            var currentLanguage = languages.FirstOrDefault(l => l.Name == currentCultureName);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find best match
            currentLanguage = languages.FirstOrDefault(l => currentCultureName.StartsWith(l.Name));
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find default language
            currentLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Get first one
            return languages[0];
        }
    }
}