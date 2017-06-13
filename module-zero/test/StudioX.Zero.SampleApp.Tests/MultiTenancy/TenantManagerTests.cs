using System.Threading.Tasks;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantManagerTests : SampleAppTestBase
    {
        private readonly TenantManager tenantManager;
        
        public TenantManagerTests()
        {
            tenantManager = Resolve<TenantManager>();
        }

        [Fact]
        public async Task ShouldNotCreateDuplicateTenant()
        {
            await tenantManager.CreateAsync(new Tenant("Tenant-X", "Tenant X"));
            
            //Trying to re-create with same tenancy name

            await Assert.ThrowsAnyAsync<StudioXException>(async () =>
            {
                await tenantManager.CreateAsync(new Tenant("Tenant-X", "Tenant X"));
            });
        }
    }
}
