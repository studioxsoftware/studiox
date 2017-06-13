using StudioX.Application.Features;
using StudioX.Domain.Repositories;
using StudioX.MultiTenancy;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Editions;

namespace StudioX.Boilerplate.MultiTenancy
{
    public class TenantManager : StudioXTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IStudioXZeroFeatureValueStore featureValueStore
            ) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore
            )
        {
        }
    }
}