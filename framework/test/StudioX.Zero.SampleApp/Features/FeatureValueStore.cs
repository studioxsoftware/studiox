using StudioX.Application.Features;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.MultiTenancy;
using StudioX.Runtime.Caching;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Features
{
    public class FeatureValueStore : StudioXFeatureValueStore<Tenant, User>
    {
        public FeatureValueStore(ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                  cacheManager, 
                  tenantFeatureRepository, 
                  tenantRepository, 
                  editionFeatureRepository, 
                  featureManager,
                  unitOfWorkManager)
        {

        }
    }
}