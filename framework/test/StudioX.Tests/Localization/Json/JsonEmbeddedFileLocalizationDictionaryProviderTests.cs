using System.Linq;
using System.Reflection;
using StudioX.Localization.Dictionaries.Json;
using StudioX.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization.Json
{
    public class JsonEmbeddedFileLocalizationDictionaryProviderTests
    {
        private readonly JsonEmbeddedFileLocalizationDictionaryProvider dictionaryProvider;

        public JsonEmbeddedFileLocalizationDictionaryProviderTests()
        {
            dictionaryProvider = new JsonEmbeddedFileLocalizationDictionaryProvider(
                typeof(JsonEmbeddedFileLocalizationDictionaryProviderTests).GetAssembly(),
                "StudioX.Tests.Localization.Json.JsonSources"
                );

            dictionaryProvider.Initialize("Lang");
        }

        [Fact]
        public void ShouldGetDictionaries()
        {
            var dictionaries = dictionaryProvider.Dictionaries.Values.ToList();

            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict["Apple"].ShouldBe("Apple");
            enDict["Banana"].ShouldBe("Banana");

            var zhCNDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "zh-CN");
            zhCNDict.ShouldNotBe(null);
            zhCNDict["Apple"].ShouldBe("苹果");
            zhCNDict["Banana"].ShouldBe("香蕉");
        }
    }
}
