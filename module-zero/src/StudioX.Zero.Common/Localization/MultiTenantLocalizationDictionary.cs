using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using StudioX.Collections.Extensions;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Localization.Dictionaries;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Session;

namespace StudioX.Localization
{
    /// <summary>
    /// Implements <see cref="IMultiTenantLocalizationDictionary"/>.
    /// </summary>
    public class MultiTenantLocalizationDictionary :
        IMultiTenantLocalizationDictionary
    {
        private readonly string sourceName;
        private readonly ILocalizationDictionary internalDictionary;
        private readonly IRepository<ApplicationLanguageText, long> customLocalizationRepository;
        private readonly ICacheManager cacheManager;
        private readonly IStudioXSession session;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionary"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionary(
            string sourceName,
            ILocalizationDictionary internalDictionary,
            IRepository<ApplicationLanguageText, long> customLocalizationRepository,
            ICacheManager cacheManager,
            IStudioXSession session,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.sourceName = sourceName;
            this.internalDictionary = internalDictionary;
            this.customLocalizationRepository = customLocalizationRepository;
            this.cacheManager = cacheManager;
            this.session = session;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public CultureInfo CultureInfo { get { return internalDictionary.CultureInfo; } }

        public string this[string name]
        {
            get => internalDictionary[name];
            set => internalDictionary[name] = value;
        }

        public LocalizedString GetOrNull(string name)
        {
            return GetOrNull(session.TenantId, name);
        }

        public LocalizedString GetOrNull(int? tenantId, string name)
        {
            //Get cache
            var cache = cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Get for current tenant
            var dictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            var value = dictionary.GetOrDefault(name);
            if (value != null)
            {
                return new LocalizedString(name, value, CultureInfo);
            }

            //Fall back to host
            if (tenantId != null)
            {
                dictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                value = dictionary.GetOrDefault(name);
                if (value != null)
                {
                    return new LocalizedString(name, value, CultureInfo);
                }
            }

            //Not found in database, fall back to internal dictionary
            var internalLocalizedString = internalDictionary.GetOrNull(name);
            if (internalLocalizedString != null)
            {
                return internalLocalizedString;
            }

            //Not found at all
            return null;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(session.TenantId);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(int? tenantId)
        {
            //Get cache
            var cache = cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Create a temp dictionary to build (by underlying dictionary)
            var dictionary = new Dictionary<string, LocalizedString>();

            foreach (var localizedString in internalDictionary.GetAllStrings())
            {
                dictionary[localizedString.Name] = localizedString;
            }

            //Override by host
            if (tenantId != null)
            {
                var defaultDictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foreach (var keyValue in defaultDictionary)
                {
                    dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
                }
            }

            //Override by tenant
            var tenantDictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            foreach (var keyValue in tenantDictionary)
            {
                dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
            }

            return dictionary.Values.ToImmutableList();
        }

        private string CalculateCacheKey(int? tenantId)
        {
            return MultiTenantLocalizationDictionaryCacheHelper.CalculateCacheKey(tenantId, sourceName, CultureInfo.Name);
        }

        [UnitOfWork]
        protected virtual Dictionary<string, string> GetAllValuesFromDatabase(int? tenantId)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return customLocalizationRepository
                    .GetAllList(l => l.Source == sourceName && l.LanguageName == CultureInfo.Name)
                    .ToDictionary(l => l.Key, l => l.Value);
            }
        }
    }
}