using StudioX.MultiTenancy;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantStoreTests : SampleAppTestBase
    {
        private readonly ITenantStore tenantStore;

        public TenantStoreTests()
        {
            tenantStore = Resolve<ITenantStore>();
        }


        [Fact]
        public void ShouldGetTenantById()
        {
            //Act
            var tenant = tenantStore.Find(1);

            //Assert
            Assert.NotNull(tenant);
            tenant.TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }
        
        [Fact]
        public void ShouldGetTenantByName()
        {
            //Act
            var tenant = tenantStore.Find(Tenant.DefaultTenantName);

            //Assert
            Assert.NotNull(tenant);
            tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void ShouldNotGetUnknownTenant()
        {
            Assert.Null(tenantStore.Find("unknown-tenancy-name"));
        }
    }
}