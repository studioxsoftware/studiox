using System;
using System.Collections.Generic;
using AutoMapper;
using Shouldly;
using Xunit;

namespace StudioX.AutoMapper.Tests
{
    public class AutoMappingTests
    {
        private readonly IMapper mapper;

        public AutoMappingTests()
        {
            var config = new MapperConfiguration(configuration =>
            {
                configuration.CreateAutoAttributeMaps(typeof(MyClass1));
                configuration.CreateAutoAttributeMaps(typeof(MyClass2));
            });

            mapper = config.CreateMapper();
        }

        [Fact]
        public void MapNullTests()
        {
            MyClass1 obj1 = null;
            var obj2 = mapper.Map<MyClass2>(obj1);
            obj2.ShouldBe(null);
        }

        [Fact]
        public void MapNullExistingObjectTests()
        {
            MyClass1 obj1 = null;

            var obj2 = new MyClass2 { TestProp = "before map" };
            mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe("before map");
        }

        [Fact]
        public void MapToTests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = mapper.Map<MyClass2>(obj1);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = mapper.Map<MyClass3>(obj1);
            obj3.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void MapToExistingObjectTests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = new MyClass2();
            mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = new MyClass3();
            mapper.Map(obj2, obj3);
            obj3.TestProp.ShouldBe("Test value");

            Assert.ThrowsAny<Exception>(() => //Did not define reverse mapping!
            {
                mapper.Map(obj3, obj2);
            });
        }

        [Fact]
        public void MapFromTests()
        {
            var obj2 = new MyClass2 { TestProp = "Test value" };

            var obj1 = mapper.Map<MyClass1>(obj2);
            obj1.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void IgnoreMapTests()
        {
            var obj2 = new MyClass2 {TestProp = "Test value", AnotherValue = 42};
            var obj3 = mapper.Map<MyClass3>(obj2);
            obj3.TestProp.ShouldBe("Test value");
            obj3.AnotherValue.ShouldBe(0); //Ignored because of IgnoreMap attribute!
        }

        [Fact]
        public void MapToCollectionTests()
        {
            var list1 = new List<MyClass1>
                        {
                            new MyClass1 {TestProp = "Test value 1"},
                            new MyClass1 {TestProp = "Test value 2"}
                        };

            var list2 = mapper.Map<List<MyClass2>>(list1);
            list2.Count.ShouldBe(2);
            list2[0].TestProp.ShouldBe("Test value 1");
            list2[1].TestProp.ShouldBe("Test value 2");
        }

        [Fact]
        public void MapShouldSetNullExistingObjectTests()
        {
            MyClass1 obj1 = new MyClass1 { TestProp = null };
            var obj2 = new MyClass2 { TestProp = "before map" };
            mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe(null);
        }

        [Fact]
        public void ShouldMapNullableValueToNullIfItIsNullOnSource()
        {
            var obj1 = new MyClass1();
            var obj2 = mapper.Map<MyClass2>(obj1);
            obj2.NullableValue.ShouldBeNull();
        }

        [Fact]
        public void ShouldMapNullableValueToNotNullIfItIsNotNullOnSource()
        {
            var obj1 = new MyClass1 { NullableValue = 42 };
            var obj2 = mapper.Map<MyClass2>(obj1);
            obj2.NullableValue.ShouldBe(42);
        }

        [AutoMap(typeof(MyClass2), typeof(MyClass3))]
        private class MyClass1
        {
            public string TestProp { get; set; }

            public long? NullableValue { get; set; }
        }

        [AutoMapTo(typeof(MyClass3))]
        private class MyClass2
        {
            public string TestProp { get; set; }

            public long? NullableValue { get; set; }

            public int AnotherValue { get; set; }
        }

        private class MyClass3
        {
            public string TestProp { get; set; }

            [IgnoreMap]
            public int AnotherValue { get; set; }
        }
    }
}
