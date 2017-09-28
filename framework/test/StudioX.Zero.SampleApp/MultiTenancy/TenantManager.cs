using StudioX.Application.Features;
using StudioX.Domain.Repositories;
using StudioX.MultiTenancy;
using StudioX.Zero.SampleApp.Editions;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.MultiTenancy
{
    public class TenantManager : StudioXTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IStudioXZeroFeatureValueStore featureValueStore) :
            base(tenantRepository, tenantFeatureRepository, editionManager, featureValueStore)
        {
        }
    }
}
