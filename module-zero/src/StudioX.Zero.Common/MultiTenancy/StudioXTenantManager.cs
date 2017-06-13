using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Authorization.Users;
using StudioX.Collections.Extensions;
using StudioX.Domain.Repositories;
using StudioX.Domain.Services;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Localization;
using StudioX.Runtime.Caching;
using StudioX.UI;
using StudioX.Zero;

namespace StudioX.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// Implements domain logic for <see cref="StudioXTenant{TUser}"/>.
    /// </summary>
    /// <typeparam name="TTenant">Type of the application Tenant</typeparam>
    /// <typeparam name="TUser">Type of the application User</typeparam>
    public class StudioXTenantManager<TTenant, TUser> : IDomainService,
        IEventHandler<EntityChangedEventData<TTenant>>,
        IEventHandler<EntityDeletedEventData<Edition>>
        where TTenant : StudioXTenant<TUser>
        where TUser : StudioXUserBase
    {
        public StudioXEditionManager EditionManager { get; set; }

        public ILocalizationManager LocalizationManager { get; set; }

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        protected IRepository<TTenant> TenantRepository { get; set; }

        protected IRepository<TenantFeatureSetting, long> TenantFeatureRepository { get; set; }

        private readonly IStudioXZeroFeatureValueStore featureValueStore;

        public StudioXTenantManager(
            IRepository<TTenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            StudioXEditionManager editionManager,
            IStudioXZeroFeatureValueStore featureValueStore)
        {
            this.featureValueStore = featureValueStore;
            TenantRepository = tenantRepository;
            TenantFeatureRepository = tenantFeatureRepository;
            EditionManager = editionManager;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        public virtual IQueryable<TTenant> Tenants => TenantRepository.GetAll();

        public virtual async Task CreateAsync(TTenant tenant)
        {
            await ValidateTenantAsync(tenant);

            if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            await TenantRepository.InsertAsync(tenant);
        }

        public async Task UpdateAsync(TTenant tenant)
        {
            if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName && t.Id != tenant.Id) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            await TenantRepository.UpdateAsync(tenant);
        }

        public virtual async Task<TTenant> FindByIdAsync(int id)
        {
            return await TenantRepository.FirstOrDefaultAsync(id);
        }

        public virtual async Task<TTenant> GetByIdAsync(int id)
        {
            var tenant = await FindByIdAsync(id);
            if (tenant == null)
            {
                throw new StudioXException("There is no tenant with id: " + id);
            }

            return tenant;
        }

        public virtual Task<TTenant> FindByTenancyNameAsync(string tenancyName)
        {
            return TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
        }

        public virtual async Task DeleteAsync(TTenant tenant)
        {
            await TenantRepository.DeleteAsync(tenant);
        }

        public Task<string> GetFeatureValueOrNullAsync(int tenantId, string featureName)
        {
            return featureValueStore.GetValueOrNullAsync(tenantId, featureName);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int tenantId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(tenantId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(int tenantId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(tenantId, value.Name, value.Value);
            }
        }

        [UnitOfWork]
        public virtual async Task SetFeatureValueAsync(int tenantId, string featureName, string value)
        {
            await SetFeatureValueAsync(await GetByIdAsync(tenantId), featureName, value);
        }

        [UnitOfWork]
        public virtual async Task SetFeatureValueAsync(TTenant tenant, string featureName, string value)
        {
            //No need to change if it's already equals to the current value
            if (await GetFeatureValueOrNullAsync(tenant.Id, featureName) == value)
            {
                return;
            }

            //Get the current feature setting
            var currentSetting = await TenantFeatureRepository.FirstOrDefaultAsync(f => f.TenantId == tenant.Id && f.Name == featureName);

            //Get the feature
            var feature = FeatureManager.GetOrNull(featureName);
            if (feature == null)
            {
                if (currentSetting != null)
                {
                    await TenantFeatureRepository.DeleteAsync(currentSetting);
                }

                return;
            }

            //Determine default value
            var defaultValue = tenant.EditionId.HasValue
                ? (await EditionManager.GetFeatureValueOrNullAsync(tenant.EditionId.Value, featureName) ?? feature.DefaultValue)
                : feature.DefaultValue;

            //No need to store value if it's default
            if (value == defaultValue)
            {
                if (currentSetting != null)
                {
                    await TenantFeatureRepository.DeleteAsync(currentSetting);
                }

                return;
            }

            //Insert/update the feature value
            if (currentSetting == null)
            {
                await TenantFeatureRepository.InsertAsync(new TenantFeatureSetting(tenant.Id, featureName, value));
            }
            else
            {
                currentSetting.Value = value;
            }
        }

        /// <summary>
        /// Resets all custom feature settings for a tenant.
        /// Tenant will have features according to it's edition.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        public async Task ResetAllFeaturesAsync(int tenantId)
        {
            await TenantFeatureRepository.DeleteAsync(f => f.TenantId == tenantId);
        }

        protected virtual async Task ValidateTenantAsync(TTenant tenant)
        {
            await ValidateTenancyNameAsync(tenant.TenancyName);
        }

        protected virtual Task ValidateTenancyNameAsync(string tenancyName)
        {
            if (!Regex.IsMatch(tenancyName, StudioXTenant<TUser>.TenancyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidTenancyName"));
            }

            return Task.FromResult(0);
        }

        private string L(string name)
        {
            return LocalizationManager.GetString(StudioXZeroConsts.LocalizationSourceName, name);
        }

        public void HandleEvent(EntityChangedEventData<TTenant> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            CacheManager.GetTenantFeatureCache().Remove(eventData.Entity.Id);
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<Edition> eventData)
        {
            var relatedTenants = TenantRepository.GetAllList(t => t.EditionId == eventData.Entity.Id);
            foreach (var relatedTenant in relatedTenants)
            {
                relatedTenant.EditionId = null;
            }
        }
    }
}