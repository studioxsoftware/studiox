using System.Linq;
using StudioX.Localization;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Localization
{
    public class ApplicationLanguageProviderTests : SampleAppTestBase
    {
        private readonly ApplicationLanguageProvider languageProvider;
        private readonly Tenant defaultTenant;

        public ApplicationLanguageProviderTests()
        {
            defaultTenant = GetDefaultTenant();
            languageProvider = Resolve<ApplicationLanguageProvider>();
        }

        [Fact]
        public void ShouldGetLanguagesForHost()
        {
            //Arrange
            StudioXSession.TenantId = null;

            //Act
            var languages = languageProvider.GetLanguages();

            //Assert
            languages.Count.ShouldBe(3);
            languages.FirstOrDefault(l => l.Name == "en").ShouldNotBeNull();
            languages.FirstOrDefault(l => l.Name == "tr").ShouldNotBeNull();
            languages.FirstOrDefault(l => l.Name == "de").ShouldNotBeNull();
        }

        [Fact]
        public void ShouldGetLanguagesForTenant()
        {
            //Arrange
            StudioXSession.TenantId = defaultTenant.Id;

            //Act
            var languages = languageProvider.GetLanguages();

            //Assert
            languages.Count.ShouldBe(4);
            languages.FirstOrDefault(l => l.Name == "en").ShouldNotBeNull();
            languages.FirstOrDefault(l => l.Name == "tr").ShouldNotBeNull();
            languages.FirstOrDefault(l => l.Name == "de").ShouldNotBeNull();
            languages.FirstOrDefault(l => l.Name == "zh-CN").ShouldNotBeNull();
        }
    }
}
