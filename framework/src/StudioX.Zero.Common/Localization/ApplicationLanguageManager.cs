using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Configuration;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;
using System.Globalization;

namespace StudioX.Localization
{
    /// <summary>
    /// Manages host and tenant languages.
    /// </summary>
    public class ApplicationLanguageManager :
        IApplicationLanguageManager,
        IEventHandler<EntityChangedEventData<ApplicationLanguage>>,
        ISingletonDependency
    {
        /// <summary>
        /// Cache name for languages.
        /// </summary>
        public const string CacheName = "StudioXZeroLanguages";

        private ITypedCache<int, Dictionary<string, ApplicationLanguage>> LanguageListCache => cacheManager.GetCache<int, Dictionary<string, ApplicationLanguage>>(CacheName);

        private readonly IRepository<ApplicationLanguage> languageRepository;
        private readonly ICacheManager cacheManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly ISettingManager settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLanguageManager"/> class.
        /// </summary>
        public ApplicationLanguageManager(
            IRepository<ApplicationLanguage> languageRepository,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager)
        {
            this.languageRepository = languageRepository;
            this.cacheManager = cacheManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.settingManager = settingManager;
        }

        /// <summary>
        /// Gets list of all languages available to given tenant (or null for host)
        /// </summary>
        /// <param name="tenantId">TenantId or null for host</param>
        public virtual async Task<IReadOnlyList<ApplicationLanguage>> GetLanguagesAsync(int? tenantId)
        {
            return (await GetLanguageDictionary(tenantId)).Values.ToImmutableList();
        }

        /// <summary>
        /// Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        [UnitOfWork]
        public virtual async Task AddAsync(ApplicationLanguage language)
        {
            if ((await GetLanguagesAsync(language.TenantId)).Any(l => l.Name == language.Name))
            {
                throw new StudioXException("There is already a language with name = " + language.Name);
            }

            using (unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                await languageRepository.InsertAsync(language);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a language.
        /// </summary>
        /// <param name="tenantId">Tenant Id or null for host.</param>
        /// <param name="languageName">FirstName of the language.</param>
        [UnitOfWork]
        public virtual async Task RemoveAsync(int? tenantId, string languageName)
        {
            var currentLanguage = (await GetLanguagesAsync(tenantId)).FirstOrDefault(l => l.Name == languageName);
            if (currentLanguage == null)
            {
                return;
            }

            if (currentLanguage.TenantId == null && tenantId != null)
            {
                throw new StudioXException("Can not delete a host language from tenant!");
            }

            using (unitOfWorkManager.Current.SetTenantId(currentLanguage.TenantId))
            {
                await languageRepository.DeleteAsync(currentLanguage.Id);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates a language.
        /// </summary>
        [UnitOfWork]
        public virtual async Task UpdateAsync(int? tenantId, ApplicationLanguage language)
        {
            var existingLanguageWithSameName = (await GetLanguagesAsync(language.TenantId)).FirstOrDefault(l => l.Name == language.Name);
            if (existingLanguageWithSameName != null)
            {
                if (existingLanguageWithSameName.Id != language.Id)
                {
                    throw new StudioXException("There is already a language with name = " + language.Name);
                }
            }

            if (language.TenantId == null && tenantId != null)
            {
                throw new StudioXException("Can not update a host language from tenant");
            }

            using (unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                await languageRepository.UpdateAsync(language);
                await unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the default language or null for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        public virtual async Task<ApplicationLanguage> GetDefaultLanguageOrNullAsync(int? tenantId)
        {
            var defaultLanguageName = tenantId.HasValue
                ? await settingManager.GetSettingValueForTenantAsync(LocalizationSettingNames.DefaultLanguage, tenantId.Value)
                : await settingManager.GetSettingValueForApplicationAsync(LocalizationSettingNames.DefaultLanguage);

            return (await GetLanguagesAsync(tenantId)).FirstOrDefault(l => l.Name == defaultLanguageName);
        }

        /// <summary>
        /// Sets the default language for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        /// <param name="languageName">FirstName of the language.</param>
        public virtual async Task SetDefaultLanguageAsync(int? tenantId, string languageName)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(languageName);
            if (tenantId.HasValue)
            {
                await settingManager.ChangeSettingForTenantAsync(tenantId.Value, LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
            else
            {
                await settingManager.ChangeSettingForApplicationAsync(LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguage> eventData)
        {
            LanguageListCache.Remove(eventData.Entity.TenantId ?? 0);

            //Also invalidate the language script cache
            cacheManager.GetCache("StudioXLocalizationScripts").Clear();
        }

        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionary(int? tenantId)
        {
            //Creates a copy of the cached dictionary (to not modify it)
            var languageDictionary = new Dictionary<string, ApplicationLanguage>(await GetLanguageDictionaryFromCacheAsync(null));

            if (tenantId == null)
            {
                return languageDictionary;
            }

            //Override tenant languages
            foreach (var tenantLanguage in await GetLanguageDictionaryFromCacheAsync(tenantId.Value))
            {
                languageDictionary[tenantLanguage.Key] = tenantLanguage.Value;
            }

            return languageDictionary;
        }

        private Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionaryFromCacheAsync(int? tenantId)
        {
            return LanguageListCache.GetAsync(tenantId ?? 0, () => GetLanguagesFromDatabaseAsync(tenantId));
        }

        [UnitOfWork]
        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguagesFromDatabaseAsync(int? tenantId)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return (await languageRepository.GetAllListAsync()).ToDictionary(l => l.Name);
            }
        }
    }
}