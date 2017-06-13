using StudioX.Localization;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Json;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Localization.Sources;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Localization.Json
{
    public class JsonAndXmlSourceMixingTests : TestBaseWithLocalIocManager
    {
        private readonly StudioXBootstrapper bootstrapper;

        public JsonAndXmlSourceMixingTests()
        {
            LocalIocManager.Register<ILanguageManager, LanguageManager>();
            LocalIocManager.Register<ILanguageProvider, DefaultLanguageProvider>();

            bootstrapper = StudioXBootstrapper.Create<MyLangModule>(LocalIocManager);
            bootstrapper.Initialize();
        }

        [Fact]
        public void TestXmlJson()
        {
            var mananger = LocalIocManager.Resolve<LocalizationManager>();

            using (CultureInfoHelper.Use("en"))
            {
                var source = mananger.GetSource("Lang");

                source.GetString("Apple").ShouldBe("Apple");
                source.GetString("Banana").ShouldBe("Banana");
                source.GetString("ThisIsATest").ShouldBe("This is a test.");
                source.GetString("HowAreYou").ShouldBe("How are you?");
            }

            using (CultureInfoHelper.Use("zh-CN"))
            {
                var source = mananger.GetSource("Lang");

                source.GetString("Apple").ShouldBe("苹果");
                source.GetString("Banana").ShouldBe("香蕉");
                source.GetString("ThisIsATest").ShouldBe("这是一个测试.");
                source.GetString("HowAreYou").ShouldBe("你好吗?");
            }
        }
    }

    public class MyLangModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    "Lang",
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyLangModule).GetAssembly(),
                         "StudioX.Tests.Localization.Json.XmlSources"
                        )
                    )
                );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    "Lang",
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyLangModule).GetAssembly(),
                         "StudioX.Tests.Localization.Json.JsonSources"
                        )));

            
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyLangModule).GetAssembly());
        }
    }
}
