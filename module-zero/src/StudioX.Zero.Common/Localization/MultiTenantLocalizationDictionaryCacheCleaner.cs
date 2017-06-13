using StudioX.Dependency;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;

namespace StudioX.Localization
{
    /// <summary>
    /// Clears related localization cache when a <see cref="ApplicationLanguageText"/> changes.
    /// </summary>
    public class MultiTenantLocalizationDictionaryCacheCleaner : 
        ITransientDependency,
        IEventHandler<EntityChangedEventData<ApplicationLanguageText>>
    {
        private readonly ICacheManager cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionaryCacheCleaner"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionaryCacheCleaner(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguageText> eventData)
        {
            cacheManager
                .GetMultiTenantLocalizationDictionaryCache()
                .Remove(MultiTenantLocalizationDictionaryCacheHelper.CalculateCacheKey(
                    eventData.Entity.TenantId,
                    eventData.Entity.Source,
                    eventData.Entity.LanguageName)
                );
        }
    }
}