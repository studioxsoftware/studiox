using StudioX.Configuration;
using NUnit.Framework;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Configuration
{
    public class DictionaryBasedConfigTest
    {
        private readonly MyConfig config;

        public DictionaryBasedConfigTest()
        {
            config = new MyConfig();
        }

        [Fact]
        public void ShouldGetValue()
        {
            var testObject = new TestClass {Value = 42};

            config["IntValue"] = 42;
            config["StringValue"] = "Test string";
            config["ObjectValue"] = testObject;

            config["IntValue"].ShouldBe(42);
            config.Get<int>("IntValue").ShouldBe(42);

            config["StringValue"].ShouldBe("Test string");
            config.Get<string>("StringValue").ShouldBe("Test string");

            config["ObjectValue"].ShouldBeSameAs(testObject);
            config.Get<TestClass>("ObjectValue").ShouldBeSameAs(testObject);
            config.Get<TestClass>("ObjectValue").Value.ShouldBe(42);
        }

        [Fact]
        public void ShouldGetDefaultIfNoValue()
        {
            config["MyUndefinedName"].ShouldBe(null);
            config.Get<string>("MyUndefinedName").ShouldBe(null);
            config.Get<MyConfig>("MyUndefinedName").ShouldBe(null);
            config.Get<int>("MyUndefinedName").ShouldBe(0);
        }

        private class MyConfig : DictionaryBasedConfig
        {

        }

        private class TestClass
        {
            public int Value { get; set; }
        }
    }
}
