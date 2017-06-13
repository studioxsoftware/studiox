using System.Globalization;
using System.Reflection;
using StudioX.Localization;
using StudioX.TestBase;
using Shouldly;
using Xunit;

namespace StudioX.Web.Tests.Localization
{
    public class StudioXWebLocalizationTests : StudioXIntegratedTestBase<StudioXWebModule>
    {
        private readonly ILocalizationManager localizationManager;

        public StudioXWebLocalizationTests()
        {
            localizationManager = Resolve<ILocalizationManager>();
        }

        [Fact]
        public void ShouldGetLocalizedStrings()
        {
            var names = Assembly.GetAssembly(typeof(StudioXWebModule)).GetManifestResourceNames();

            var source = localizationManager.GetSource(StudioXWebConsts.LocalizaionSourceName);
            source.GetString("Yes", new CultureInfo("en-US")).ShouldBe("Yes");
            source.GetString("Yes", new CultureInfo("tr-TR")).ShouldBe("Evet");
        }
    }
}
