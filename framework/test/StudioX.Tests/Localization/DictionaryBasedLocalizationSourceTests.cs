using System.Globalization;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Xml;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization
{
    public class DictionaryBasedLocalizationSourceTests
    {
        private readonly DictionaryBasedLocalizationSource localizationSource;

        public DictionaryBasedLocalizationSourceTests()
        {
            localizationSource = new DictionaryBasedLocalizationSource("Test", new FakeLocalizationDictionary());
            localizationSource.Initialize(new LocalizationConfiguration
            {
                HumanizeTextIfNotFound = false,
                WrapGivenTextIfNotFound = true
            }, new IocManager());
        }

        [Fact]
        public void ShouldGetCorrectStringOnExactCulture()
        {
            Assert.Equal("Yeryüzü", localizationSource.GetString("world", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void ShouldGetMostCloseStringOnBaseCulture()
        {
            Assert.Equal("Merhaba", localizationSource.GetString("hello", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void ShouldGetDefaultIfNotExistsOnGivenCulture()
        {
            Assert.Equal("Fourty Two (42)", localizationSource.GetString("fourtyTwo", new CultureInfo("tr")));
            Assert.Equal("Fourty Two (42)", localizationSource.GetString("fourtyTwo", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void ShouldGetAllStrings()
        {
            var localizedStrings = localizationSource.GetAllStrings(new CultureInfo("tr-TR")).OrderBy(ls => ls.Name).ToList();
            Assert.Equal(3, localizedStrings.Count);
            Assert.Equal("Fourty Two (42)", localizedStrings[0].Value);
            Assert.Equal("Merhaba", localizedStrings[1].Value);
            Assert.Equal("Yeryüzü", localizedStrings[2].Value);
        }

        [Fact]
        public void ShouldExtendLocalizationSourceOverriding()
        {
            localizationSource.Extend(
                new LocalizationDictionaryWithAddMethod(new CultureInfo("tr"))
                {
                    {"hello", "Selam"},
                });

            localizationSource.GetString("hello", new CultureInfo("tr-TR")).ShouldBe("Selam");
        }

        [Fact]
        public void ShouldExtendLocalizationSourceWithNewLanguage()
        {
            localizationSource.Extend(
                new LocalizationDictionaryWithAddMethod(new CultureInfo("fr"))
                {
                    {"hello", "Bonjour"},
                });

            localizationSource.GetString("hello", new CultureInfo("fr")).ShouldBe("Bonjour");
            localizationSource.GetString("world", new CultureInfo("fr")).ShouldBe("World"); //not localed into french
        }

        [Fact]
        public void ShouldReturnGivenTextIfNotFound()
        {
            localizationSource.GetString("An undefined text").ShouldBe("[An undefined text]");
        }

        private class FakeLocalizationDictionary : LocalizationDictionaryProviderBase
        {
            public FakeLocalizationDictionary()
            {
                Dictionaries["en"] = new LocalizationDictionaryWithAddMethod(new CultureInfo("en"))
            {
                {"hello", "Hello"},
                {"world", "World"},
                {"fourtyTwo", "Fourty Two (42)"}
            };

                Dictionaries["tr"] = new LocalizationDictionaryWithAddMethod(new CultureInfo("tr"))
            {
                {"hello", "Merhaba"},
                {"world", "Dünya"}
            };

                Dictionaries["tr-TR"] = new LocalizationDictionaryWithAddMethod(new CultureInfo("tr-TR"))
            {
                {"world", "Yeryüzü"}
            };


                DefaultDictionary = Dictionaries["en"];
            }
        }
    }
}