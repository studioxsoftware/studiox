using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace StudioX.Localization.Dictionaries
{
    /// <summary>
    ///     Represents a simple implementation of <see cref="ILocalizationDictionary" /> interface.
    /// </summary>
    public class LocalizationDictionary : ILocalizationDictionary, IEnumerable<LocalizedString>
    {
        /// <inheritdoc />
        public CultureInfo CultureInfo { get; }

        /// <inheritdoc />
        public virtual string this[string name]
        {
            get
            {
                var localizedString = GetOrNull(name);
                return localizedString?.Value;
            }
            set { dictionary[name] = new LocalizedString(name, value, CultureInfo); }
        }

        private readonly Dictionary<string, LocalizedString> dictionary;

        /// <summary>
        ///     Creates a new <see cref="LocalizationDictionary" /> object.
        /// </summary>
        /// <param name="cultureInfo">Culture of the dictionary</param>
        public LocalizationDictionary(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
            dictionary = new Dictionary<string, LocalizedString>();
        }

        /// <inheritdoc />
        public virtual LocalizedString GetOrNull(string name)
        {
            LocalizedString localizedString;
            return dictionary.TryGetValue(name, out localizedString) ? localizedString : null;
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return dictionary.Values.ToImmutableList();
        }

        /// <inheritdoc />
        public virtual IEnumerator<LocalizedString> GetEnumerator()
        {
            return GetAllStrings().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetAllStrings().GetEnumerator();
        }

        protected bool Contains(string name)
        {
            return dictionary.ContainsKey(name);
        }
    }
}