using System.Linq;
using System.Threading.Tasks;
using StudioX.Localization;
using StudioX.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Localization
{
    public class ApplicationLanguageManagerTests : SampleAppTestBase
    {
        private readonly Tenant defaultTenant;
        private readonly IApplicationLanguageManager languageManager;

        public ApplicationLanguageManagerTests()
        {
            defaultTenant = GetDefaultTenant();
            languageManager = Resolve<IApplicationLanguageManager>();
        }

        [Fact]
        public async Task ShouldGetAllHostLanguages()
        {
            var languages = await languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(3);
        }

        [Fact]
        public async Task ShouldGetAllTenantLanguages()
        {
            var languages = await languageManager.GetLanguagesAsync(defaultTenant.Id);
            languages.Count.ShouldBe(4);
        }

        [Fact]
        public async Task ShouldAddLanguageToHost()
        {
            await languageManager.AddAsync(new ApplicationLanguage(null, "fr", "French"));

            var languages = await languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(4);
            languages.FirstOrDefault(l => l.Name == "fr").ShouldNotBeNull();
        }

        [Fact]
        public async Task ShouldAddLanguageToTenant()
        {
            await languageManager.AddAsync(new ApplicationLanguage(defaultTenant.Id, "fr", "French"));

            var languages = await languageManager.GetLanguagesAsync(defaultTenant.Id);
            languages.Count.ShouldBe(5);
            languages.FirstOrDefault(l => l.Name == "fr").ShouldNotBeNull();
        }

        [Fact]
        public async Task RemoveLanguageFromHost()
        {
            await languageManager.RemoveAsync(null, "tr");

            var languages = await languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(2);
            languages.FirstOrDefault(l => l.Name == "tr").ShouldBeNull();
        }

        [Fact]
        public async Task RemoveLanguageFromTenant()
        {
            await languageManager.RemoveAsync(null, "tr");

            var languages = await languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(2);
            languages.FirstOrDefault(l => l.Name == "tr").ShouldBeNull();
        }
    }
}