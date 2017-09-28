using StudioX.Domain.Repositories;
using StudioX.MultiTenancy;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantCacheTests : SampleAppTestBase
    {
        private readonly ITenantCache tenantCache;
        private readonly IRepository<Tenant> tenantRepository;

        public TenantCacheTests()
        {
            tenantCache = Resolve<ITenantCache>();
            tenantRepository = Resolve<IRepository<Tenant>>();
        }

        [Fact]
        public void ShouldGetTenantById()
        {
            //Act
            var tenant = tenantCache.Get(1);

            //Assert
            tenant.TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }
        
        [Fact]
        public void ShouldGetTenantByTenancyName()
        {
            //Act
            var tenant = tenantCache.GetOrNull(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void ShouldGetNullTenantByTenancyNameIfNotFound()
        {
            //Act
            var tenant = tenantCache.GetOrNull("unknown-tenancy-name");

            //Assert
            tenant.ShouldBeNull();
        }
        
        [Fact]
        public void ShouldRefreshCacheIfTenancyNameChanges()
        {
            // Get a known tenant from cache

            //Act
            var tenant = tenantCache.GetOrNull(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeTrue();

            // Change tenant name

            tenantRepository.Update(tenant.Id, t =>
            {
                t.TenancyName = "Default-Changed";
                t.IsActive = false;
            });

            // Get again from cache

            //Can not get with old name
            tenant = tenantCache.GetOrNull(Tenant.DefaultTenantName);
            tenant.ShouldBeNull();

            //Can get with new name
            tenant = tenantCache.GetOrNull("Default-Changed");
            tenant.ShouldNotBeNull();
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeFalse();
        }
    }
}