using System.Collections.Generic;
using StudioX.Localization.Sources;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    /// A specialized list to store <see cref="ILocalizationSource"/> object.
    /// </summary>
    internal class LocalizationSourceList : List<ILocalizationSource>, ILocalizationSourceList
    {
        public IList<LocalizationSourceExtensionInfo> Extensions { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationSourceList()
        {
            Extensions = new List<LocalizationSourceExtensionInfo>();
        }
    }
}