using System.Threading.Tasks;
using StudioX.Application.Editions;
using StudioX.Authorization.Users;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.MultiTenancy;
using StudioX.Runtime.Caching;

namespace StudioX.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureValueStore"/>.
    /// </summary>
    public class StudioXFeatureValueStore<TTenant, TUser> : 
        IStudioXZeroFeatureValueStore, 
        ITransientDependency,
        IEventHandler<EntityChangedEventData<Edition>>,
        IEventHandler<EntityChangedEventData<EditionFeatureSetting>>

        where TTenant : StudioXTenant<TUser>
        where TUser : StudioXUserBase
    {
        private readonly ICacheManager cacheManager;
        private readonly IRepository<TenantFeatureSetting, long> tenantFeatureRepository;
        private readonly IRepository<TTenant> tenantRepository;
        private readonly IRepository<EditionFeatureSetting, long> editionFeatureRepository;
        private readonly IFeatureManager featureManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXFeatureValueStore{TTenant, TUser}"/> class.
        /// </summary>
        public StudioXFeatureValueStore(
            ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IRepository<TTenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.cacheManager = cacheManager;
            this.tenantFeatureRepository = tenantFeatureRepository;
            this.tenantRepository = tenantRepository;
            this.editionFeatureRepository = editionFeatureRepository;
            this.featureManager = featureManager;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetValueOrNullAsync(int tenantId, Feature feature)
        {
            return GetValueOrNullAsync(tenantId, feature.Name);
        }

        public virtual async Task<string> GetEditionValueOrNullAsync(int editionId, string featureName)
        {
            var cacheItem = await GetEditionFeatureCacheItemAsync(editionId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual async Task<string> GetValueOrNullAsync(int tenantId, string featureName)
        {
            var cacheItem = await GetTenantFeatureCacheItemAsync(tenantId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            if (value != null)
            {
                return value;
            }

            if (cacheItem.EditionId.HasValue)
            {
                value = await GetEditionValueOrNullAsync(cacheItem.EditionId.Value, featureName);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        [UnitOfWork]
        public virtual async Task SetEditionFeatureValueAsync(int editionId, string featureName, string value)
        {
            if (await GetEditionValueOrNullAsync(editionId, featureName) == value)
            {
                return;
            }

            var currentFeature = await editionFeatureRepository.FirstOrDefaultAsync(f => f.EditionId == editionId && f.Name == featureName);

            var feature = featureManager.GetOrNull(featureName);
            if (feature == null || feature.DefaultValue == value)
            {
                if (currentFeature != null)
                {
                    await editionFeatureRepository.DeleteAsync(currentFeature);
                }

                return;
            }

            if (currentFeature == null)
            {
                await editionFeatureRepository.InsertAsync(new EditionFeatureSetting(editionId, featureName, value));
            }
            else
            {
                currentFeature.Value = value;
            }
        }

        protected virtual async Task<TenantFeatureCacheItem> GetTenantFeatureCacheItemAsync(int tenantId)
        {
            return await cacheManager.GetTenantFeatureCache().GetAsync(tenantId, async () =>
            {
                TTenant tenant;
                using (var uow = unitOfWorkManager.Begin())
                {
                    using (unitOfWorkManager.Current.SetTenantId(null))
                    {
                        tenant = await tenantRepository.GetAsync(tenantId);

                        await uow.CompleteAsync();
                    }
                }

                var newCacheItem = new TenantFeatureCacheItem { EditionId = tenant.EditionId };

                using (var uow = unitOfWorkManager.Begin())
                {
                    using (unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var featureSettings = await tenantFeatureRepository.GetAllListAsync();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        await uow.CompleteAsync();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual async Task<EditionfeatureCacheItem> GetEditionFeatureCacheItemAsync(int editionId)
        {
            return await cacheManager
                .GetEditionFeatureCache()
                .GetAsync(
                    editionId,
                    async () => await CreateEditionFeatureCacheItem(editionId)
                );
        }

        protected virtual async Task<EditionfeatureCacheItem> CreateEditionFeatureCacheItem(int editionId)
        {
            var newCacheItem = new EditionfeatureCacheItem();

            var featureSettings = await editionFeatureRepository.GetAllListAsync(f => f.EditionId == editionId);
            foreach (var featureSetting in featureSettings)
            {
                newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
            }

            return newCacheItem;
        }

        public virtual void HandleEvent(EntityChangedEventData<EditionFeatureSetting> eventData)
        {
            cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.EditionId);
        }

        public virtual void HandleEvent(EntityChangedEventData<Edition> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.Id);
        }
    }
}