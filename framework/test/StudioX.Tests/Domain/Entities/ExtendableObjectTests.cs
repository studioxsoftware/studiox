using System;
using System.Collections.Generic;
using Shouldly;
using StudioX.Domain.Entities;
using StudioX.Extensions;
using StudioX.Json;
using Xunit;

namespace StudioX.Tests.Domain.Entities
{
    public class ExtendableObjectTests
    {
        [Fact]
        public void ShouldSetAndGetPrimitiveValues()
        {
            var entity = new MyEntity();

            entity.SetData("Name", "John");
            entity.GetData<string>("Name").ShouldBe("John");

            entity.SetData("Length", 42424242);
            entity.GetData<int>("Length").ShouldBe(42424242);

            entity.SetData("Age", 42);
            Assert.Equal(42, entity.GetData<byte>("Age"));

            entity.SetData("BirthDate", new DateTime(2015, 05, 22, 13, 24, 00, DateTimeKind.Utc));
            Assert.Equal(new DateTime(2015, 05, 22, 13, 24, 00, DateTimeKind.Utc),
                entity.GetData<DateTime>("BirthDate"));

            entity.SetData("EnumVal", MyEnum.Value2);
            entity.GetData<MyEnum>("EnumVal").ShouldBe(MyEnum.Value2);

            entity.GetData<string>("NonExistingValue").ShouldBe(null);
        }

        [Fact]
        public void ShouldSetAndGetComplexValues()
        {
            var entity = new MyEntity();

            var obj = new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "B", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            };

            entity.SetData("ComplexData", obj);
            var obj2 = entity.GetData<MyComplexType>("ComplexData");

            obj.ToJsonString().ShouldBe(obj2.ToJsonString());

            entity.SetData("ComplexData", (MyComplexType) null);
            entity.GetData<MyComplexType>("ComplexData").ShouldBe(null);
        }

        [Fact]
        public void ShouldSetExtensionDataToNullIfNoPropertiesRemainWithSettingPropertiesToDefault()
        {
            var entity = new MyEntity();

            entity.ExtensionData.ShouldBeNull(); //It's null at the beginning

            entity.SetData("Name", "Douglas");
            entity.SetData("Age", 42);
            entity.ExtensionData.ShouldNotBeNull();

            entity.SetData<string>("Name", null); //setting to default removes data
            entity.ExtensionData.ShouldNotBeNull(); //but there is an "Age" property.
            entity.SetData("Age", 0); //setting to default removes data, no data remains
            entity.ExtensionData.ShouldBeNull(); //Now, it's null
        }

        [Fact]
        public void ShouldSetExtensionDataToNullIfNoPropertiesRemainWithRemovingProperties()
        {
            var entity = new MyEntity();

            entity.ExtensionData.ShouldBeNull(); //It's null at the beginning

            entity.SetData("Name", "Douglas");
            entity.SetData("Age", 42);
            entity.ExtensionData.ShouldNotBeNull();

            entity.RemoveData("Name");
            entity.ExtensionData.ShouldNotBeNull();
            entity.RemoveData("Age");
            entity.ExtensionData.ShouldBeNull(); //Now, it's null
        }

        [Fact]
        public void ShouldGetDefaultIfNotPresent()
        {
            var entity = new MyEntity();
            entity.GetData<string>("Name").ShouldBe(null);
            entity.GetData<int>("Length").ShouldBe(0);
            entity.GetData<int?>("Length").ShouldBe(null);
            entity.GetData<DateTime>("BirthDate").ShouldBe(new DateTime());
            entity.GetData<DateTime?>("BirthDate").ShouldBe(null);
            Assert.Equal(0, entity.GetData<byte>("Age"));
            Assert.Equal(null, entity.GetData<byte?>("Age"));
            entity.GetData<MyComplexType>("ComplexData").ShouldBe(null);
        }

        [Fact]
        public void ShouldSupportInheritanceOfComplexObjects()
        {
            var entity = new MyEntity();
            entity.SetData<IAnimal>("MyCat", new Tiger(), true);
            var tiger = entity.GetData<IAnimal>("MyCat", true) as Tiger;
            tiger.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldSupportInheritanceOfComplexObjectsInsideArray()
        {
            var entity = new MyEntity();

            var animals = new AnimalBase[]
            {
                new Cat {Friend = new Lion()},
                new Lion(),
                new Tiger()
            };

            entity.SetData("MyAnimals", animals, true);
            var animals2 = entity.GetData<AnimalBase[]>("MyAnimals", true);

            animals2.Length.ShouldBe(3);
            animals2[0].ShouldBeOfType<Cat>();
            animals2[0].As<Cat>().Friend.ShouldBeOfType<Lion>();
            animals2[1].ShouldBeOfType<Lion>();
            animals2[2].ShouldBeOfType<Tiger>();
        }

        private class MyEntity : Entity, IExtendableObject
        {
            public string ExtensionData { get; set; }
        }

        public class MyComplexType
        {
            public string Name { get; set; }
            public byte Age { get; set; }
            public List<MyComplexTypeInner> Inner { get; set; }
        }

        public class MyComplexTypeInner
        {
            public string Value1 { get; set; }
            public int? Value2 { get; set; }
        }

        public interface IAnimal
        {
            string Name { get; }
        }

        public abstract class AnimalBase : IAnimal
        {
            public string Name => GetType().Name;
        }

        public class Cat : AnimalBase
        {
            public IAnimal Friend { get; set; }

            public virtual void CatMethod()
            {
            }
        }

        public class Lion : AnimalBase
        {
            public void LionMethod()
            {
            }
        }

        public class Tiger : Cat
        {
            public void TigerMethod()
            {
            }
        }

        public enum MyEnum
        {
            Value1,
            Value2
        }
    }
}