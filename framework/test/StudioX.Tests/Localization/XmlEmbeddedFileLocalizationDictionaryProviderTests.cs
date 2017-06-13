using System.Linq;
using System.Reflection;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization
{
    public class XmlEmbeddedFileLocalizationDictionaryProviderTests
    {
        private readonly XmlEmbeddedFileLocalizationDictionaryProvider dictionaryProvider;

        public XmlEmbeddedFileLocalizationDictionaryProviderTests()
        {
            dictionaryProvider = new XmlEmbeddedFileLocalizationDictionaryProvider(
                typeof(XmlEmbeddedFileLocalizationDictionaryProviderTests).GetAssembly(),
                "StudioX.Tests.Localization.TestXmlFiles"
                );

            dictionaryProvider.Initialize("Test");
        }

        [Fact]
        public void ShouldGetDictionaries()
        {
            var dictionaries = dictionaryProvider.Dictionaries.Values.ToList();
            
            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict.ShouldBe(dictionaryProvider.DefaultDictionary);
            enDict["hello"].ShouldBe("Hello");
            
            var trDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "tr");
            trDict.ShouldNotBe(null);
            trDict["hello"].ShouldBe("Merhaba");
        }
    }
}
