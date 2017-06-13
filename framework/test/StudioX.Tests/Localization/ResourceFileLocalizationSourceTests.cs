using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.Localization.Sources.Resource;
using StudioX.Tests.Localization.TestResourceFiles;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization
{
    public class ResourceFileLocalizationSourceTests
    {
        private readonly ResourceFileLocalizationSource resourceFileLocalizationSource;

        public ResourceFileLocalizationSourceTests()
        {
            resourceFileLocalizationSource = new ResourceFileLocalizationSource("MyTestResource", MyTestResource.ResourceManager);
            resourceFileLocalizationSource.Initialize(new LocalizationConfiguration
            {
                HumanizeTextIfNotFound = false,
                WrapGivenTextIfNotFound = true
            }, new IocManager());
        }

        [Fact]
        public void TestGetString()
        {
            //Defined in English
            resourceFileLocalizationSource.GetString("Hello", CultureInfoHelper.Get("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            resourceFileLocalizationSource.GetString("Hello", CultureInfoHelper.Get("en-US")).ShouldBe("Hello!");
            resourceFileLocalizationSource.GetString("World", CultureInfoHelper.Get("en-US")).ShouldBe("World!");
            resourceFileLocalizationSource.GetString("Hello", CultureInfoHelper.Get("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            resourceFileLocalizationSource.GetString("Hello", CultureInfoHelper.Get("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            resourceFileLocalizationSource.GetString("Hello", CultureInfoHelper.Get("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            resourceFileLocalizationSource.GetString("World", CultureInfoHelper.Get("tr-TR")).ShouldBe("World!");

            //Undefined at all, fallback to given text
            resourceFileLocalizationSource.GetString("Apple", CultureInfoHelper.Get("en-US")).ShouldBe("[Apple]");
        }

        [Fact]
        public void TestGetStringOrNull()
        {
            //Defined in English
            resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfoHelper.Get("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfoHelper.Get("en-US")).ShouldBe("Hello!");
            resourceFileLocalizationSource.GetStringOrNull("World", CultureInfoHelper.Get("en-US")).ShouldBe("World!");
            resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfoHelper.Get("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfoHelper.Get("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfoHelper.Get("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            resourceFileLocalizationSource.GetStringOrNull("World", CultureInfoHelper.Get("tr-TR")).ShouldBe("World!");

            //Undefined at all, returns null
            resourceFileLocalizationSource.GetStringOrNull("Apple", CultureInfoHelper.Get("en-US")).ShouldBeNull();
        }

        //[Fact] 
        public void TestGetAllStrings()
        {
            var allStrings = resourceFileLocalizationSource.GetAllStrings(CultureInfoHelper.Get("en"));
            allStrings.Count.ShouldBe(2);
            allStrings.Any(s => s.Name == "Hello" && s.Value == "Hello!").ShouldBeTrue();
            allStrings.Any(s => s.Name == "World" && s.Value == "World!").ShouldBeTrue();
        }
    }
}