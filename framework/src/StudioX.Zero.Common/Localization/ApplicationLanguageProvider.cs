using System.Collections.Generic;
using System.Linq;
using StudioX.Runtime.Session;
using StudioX.Threading;

namespace StudioX.Localization
{
    /// <summary>
    /// Implements <see cref="ILanguageProvider"/> to get languages from <see cref="IApplicationLanguageManager"/>.
    /// </summary>
    public class ApplicationLanguageProvider : ILanguageProvider
    {
        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        private readonly IApplicationLanguageManager applicationLanguageManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApplicationLanguageProvider(IApplicationLanguageManager applicationLanguageManager)
        {
            this.applicationLanguageManager = applicationLanguageManager;

            StudioXSession = NullStudioXSession.Instance;
        }

        /// <summary>
        /// Gets the languages for current tenant.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            var languageInfos = AsyncHelper.RunSync(() => applicationLanguageManager.GetLanguagesAsync(StudioXSession.TenantId))
                    .OrderBy(l => l.DisplayName)
                    .Select(l => l.ToLanguageInfo())
                    .ToList();

            SetDefaultLanguage(languageInfos);

            return languageInfos;
        }

        private void SetDefaultLanguage(List<LanguageInfo> languageInfos)
        {
            if (languageInfos.Count <= 0)
            {
                return;
            }

            var defaultLanguage = AsyncHelper.RunSync(() => applicationLanguageManager.GetDefaultLanguageOrNullAsync(StudioXSession.TenantId));
            if (defaultLanguage == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }
            
            var languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
            if (languageInfo == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            languageInfo.IsDefault = true;
        }
    }
}