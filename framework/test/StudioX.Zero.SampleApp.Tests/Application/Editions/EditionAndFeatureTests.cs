using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Zero.SampleApp.Editions;
using StudioX.Zero.SampleApp.Features;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Application.Editions
{
    public class EditionAndFeatureTests : SampleAppTestBase
    {
        private readonly EditionManager editionManager;
        private readonly TenantManager tenantManager;
        private readonly IFeatureChecker featureChecker;

        public EditionAndFeatureTests()
        {
            editionManager = Resolve<EditionManager>();
            tenantManager = Resolve<TenantManager>();
            featureChecker = Resolve<FeatureChecker>();
        }

        [Fact]
        public async Task ShouldCreateEdition()
        {
            await editionManager.CreateAsync(new Edition { Name = "Standard", DisplayName = "Standard Edition" });

            UsingDbContext(context =>
            {
                context.Editions.FirstOrDefault(e => e.Name == "Standard").ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ShouldDeleteEdition()
        {
            var standardEdition = await CreateEditionAsync("Standard");

            await editionManager.DeleteAsync(standardEdition);

            UsingDbContext(context =>
            {
                context.Editions.FirstOrDefault(e => e.Name == "Standard").ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ComplexTestScenario()
        {
            var standardEdition = await CreateEditionAsync("Standard");
            var defaultTenant = GetDefaultTenant();

            defaultTenant.EditionId = standardEdition.Id;
            await tenantManager.UpdateAsync(defaultTenant);

            StudioXSession.TenantId = defaultTenant.Id;

            //No value initially
            (await editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature)).ShouldBeNull();
            (await editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature)).ShouldBeNull();
            (await tenantManager.GetFeatureValueOrNullAsync(defaultTenant.Id, AppFeatureProvider.MyBoolFeature)).ShouldBeNull();
            (await tenantManager.GetFeatureValueOrNullAsync(defaultTenant.Id, AppFeatureProvider.MyNumericFeature)).ShouldBeNull();

            //Should get default values
            (await featureChecker.IsEnabledAsync(AppFeatureProvider.MyBoolFeature)).ShouldBeFalse();
            (await featureChecker.GetValueAsync(AppFeatureProvider.MyNumericFeature)).ShouldBe("42");

            //Set edition values
            await editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature, "true");
            await editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature, "43");

            //Should get new values for edition
            (await editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature)).ShouldBe("43");
            (await editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature)).ShouldBe("true");

            //Should get edition values for tenant
            (await featureChecker.GetValueAsync(AppFeatureProvider.MyNumericFeature)).ShouldBe("43");
            (await featureChecker.IsEnabledAsync(AppFeatureProvider.MyBoolFeature)).ShouldBeTrue();
        }

        private async Task<Edition> CreateEditionAsync(string name)
        {
            UsingDbContext(context => { context.Editions.Add(new Edition { Name = name, DisplayName = name + " Edition" }); });

            var standardEdition = await editionManager.FindByNameAsync("Standard");
            standardEdition.ShouldNotBeNull();

            return standardEdition;
        }
    }
}
