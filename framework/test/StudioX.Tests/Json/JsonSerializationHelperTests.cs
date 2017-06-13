using System;
using StudioX.Json;
using StudioX.Localization;
using StudioX.Timing;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Json
{
    public class JsonSerializationHelperTests
    {
        [Fact]
        public void ShouldSimplySerializeAndDeserialize()
        {
            var str = JsonSerializationHelper.SerializeWithType(new LocalizableString("Foo", "Bar"));
            var result = (LocalizableString)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Foo");
            result.SourceName.ShouldBe("Bar");
        }

        [Fact]
        public void ShouldDeserializeWithDifferentAssemblyVersion()
        {
            var str = "StudioX.Localization.LocalizableString, StudioX, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null|{\"SourceName\":\"Bar\",\"Name\":\"Foo\"}";
            var result = (LocalizableString)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Foo");
            result.SourceName.ShouldBe("Bar");
        }

        [Fact]
        public void ShouldDeserializeWithDateTime()
        {
            Clock.Provider = ClockProviders.Utc;

            var str = "StudioX.Tests.Json.JsonSerializationHelperTests+MyClass2, StudioX.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null|{\"Date\":\"2016-04-13T16:58:10.526+08:00\"}";
            var result = (MyClass2)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Date.ShouldBe(new DateTime(2016, 04, 13, 08, 58, 10, 526, Clock.Kind));
            result.Date.Kind.ShouldBe(Clock.Kind);
        }

        public class MyClass1
        {
            public string Name { get; set; }

            public MyClass1()
            {

            }

            public MyClass1(string name)
            {
                Name = name;
            }
        }

        public class MyClass2
        {
            public DateTime Date { get; set; }

            public MyClass2(DateTime date)
            {
                Date = date;
            }
        }
    }
}