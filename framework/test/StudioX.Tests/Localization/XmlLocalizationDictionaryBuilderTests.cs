using StudioX.Localization.Dictionaries.Xml;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization
{
    public class XmlLocalizationDictionaryBuilderTests
    {
        [Fact]
        public void CanBuildLocalizationDictionaryFromXmlString()
        {
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                <localizationDictionary culture=""tr"">
                                  <texts>
                                    <text name=""hello"" value=""Merhaba"" />
                                    <text name=""world"">Dünya</text>
                                  </texts>
                                </localizationDictionary>";

            var dictionary = XmlLocalizationDictionary.BuildFomXmlString(xmlString);

            dictionary.CultureInfo.Name.ShouldBe("tr");
            dictionary["hello"].ShouldBe("Merhaba");
            dictionary["world"].ShouldBe("Dünya");
        }

        [Fact]
        public void ShouldThrowExceptionForDuplicateName()
        {
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                <localizationDictionary culture=""tr"">
                                  <texts>
                                    <text name=""hello"" value=""Merhaba"" />
                                    <text name=""hello"" value=""Merhabalar""></text>
                                  </texts>
                                </localizationDictionary>";

            Assert.Throws<StudioXException>(() => XmlLocalizationDictionary.BuildFomXmlString(xmlString));
        }
    }
}