using System.Collections.Generic;
using System.Globalization;
using StudioX.Configuration.Startup;
using StudioX.Dependency;

namespace StudioX.Localization.Sources
{
    /// <summary>
    /// Null object pattern for <see cref="ILocalizationSource"/>.
    /// </summary>
    internal class NullLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullLocalizationSource Instance => SingletonInstance;

        private static readonly NullLocalizationSource SingletonInstance = new NullLocalizationSource();

        public string Name => null;

        private readonly IReadOnlyList<LocalizedString> emptyStringArray = new LocalizedString[0];

        private NullLocalizationSource()
        {
            
        }

        public void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            
        }

        public string GetString(string name)
        {
            return name;
        }

        public string GetString(string name, CultureInfo culture)
        {
            return name;
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            return null;
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            return null;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            return emptyStringArray;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            return emptyStringArray;
        }
    }
}