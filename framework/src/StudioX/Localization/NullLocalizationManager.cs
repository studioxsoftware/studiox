using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using StudioX.Localization.Sources;

namespace StudioX.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get; } = new NullLocalizationManager();

        public LanguageInfo CurrentLanguage => new LanguageInfo(CultureInfo.CurrentUICulture.Name, CultureInfo.CurrentUICulture.DisplayName);

        private readonly IReadOnlyList<LanguageInfo> emptyLanguageArray = new LanguageInfo[0];

        private readonly IReadOnlyList<ILocalizationSource> emptyLocalizationSourceArray = new ILocalizationSource[0];

        private NullLocalizationManager()
        {
            
        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return emptyLanguageArray;
        }

        public ILocalizationSource GetSource(string name)
        {
            return NullLocalizationSource.Instance;
        }

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return emptyLocalizationSourceArray;
        }
    }
}