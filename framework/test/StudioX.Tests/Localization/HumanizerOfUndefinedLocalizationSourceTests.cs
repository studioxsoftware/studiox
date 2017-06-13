using System.Globalization;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.Localization.Sources.Resource;
using StudioX.Tests.Localization.TestResourceFiles;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization
{
    public class HumanizerOfUndefinedLocalizationSourceTests
    {
        private readonly ResourceFileLocalizationSource resourceFileLocalizationSource;
        private LocalizationConfiguration localizationConfiguration;

        public HumanizerOfUndefinedLocalizationSourceTests()
        {
            localizationConfiguration = new LocalizationConfiguration { WrapGivenTextIfNotFound = false };
            resourceFileLocalizationSource = new ResourceFileLocalizationSource("MyTestResource", MyTestResource.ResourceManager);
            resourceFileLocalizationSource.Initialize(localizationConfiguration, new IocManager());
        }

        [Fact]
        public void UndefinedLocalizationSourceShouldBeHumanized()
        {
            // Fallback to the same text as It's already in sentence case
            resourceFileLocalizationSource
                .GetString("Lorem ipsum dolor sit amet", new CultureInfo("en-US"))
            .ShouldBe("Lorem ipsum dolor sit amet");

            // Text in PascalCase should be converted properly
            resourceFileLocalizationSource
                .GetString("LoremIpsumDolorSitAmet", new CultureInfo("en-US"))
                .ShouldBe("Lorem ipsum dolor sit amet");

            // Text with mixed cases should be converted properly
            resourceFileLocalizationSource
                .GetString("LoremIpsum dolor sit amet", new CultureInfo("en-US"))
                .ShouldBe("Lorem ipsum dolor sit amet");
        }
    }
}
