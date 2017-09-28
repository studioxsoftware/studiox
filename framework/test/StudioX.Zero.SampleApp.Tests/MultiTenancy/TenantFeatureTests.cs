using StudioX.Application.Features;
using StudioX.MultiTenancy;
using StudioX.Zero.SampleApp.Features;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantFeatureTests : SampleAppTestBase
    {
        private readonly TenantManager tenantManager;
        private readonly IFeatureChecker featureChecker;

        public TenantFeatureTests()
        {
            tenantManager = Resolve<TenantManager>();
            featureChecker = Resolve<IFeatureChecker>();
        }

        [Fact]
        public void ChangingTenantFeatureShouldNotEffectOtherTenants()
        {
            //Create tenants
            var firstTenantId = UsingDbContext(context =>
            {
                var firstTenant = new Tenant("Tenant1", "Tenant1");
                context.Tenants.Add(firstTenant);
                context.SaveChanges();
                return firstTenant.Id;
            });

            var secondTenantId = UsingDbContext(context =>
            {
                var secondTenant = new Tenant("Tenant2", "Tenant2");
                context.Tenants.Add(secondTenant);
                context.SaveChanges();
                return secondTenant.Id;
            });
            
            tenantManager.SetFeatureValue(firstTenantId, AppFeatureProvider.MyBoolFeature, "true");
            featureChecker.IsEnabled(secondTenantId, AppFeatureProvider.MyBoolFeature).ShouldBe(false);
        }
    }
}
