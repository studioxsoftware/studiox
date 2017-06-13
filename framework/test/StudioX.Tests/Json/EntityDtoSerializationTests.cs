using Shouldly;
using StudioX.Application.Services.Dto;
using StudioX.Json;
using Xunit;

namespace StudioX.Tests.Json
{
    public class EntityDtoSerializationTests
    {
        [Fact]
        public void ShouldSerializeTypesDerivedFromEntityDto()
        {
            var obj = new MyClass1
            {
                Id = 42,
                Value = new MyClass2
                {
                    Id = 42
                }
            };

            obj.ToJsonString().ShouldNotBeNull();
        }

        public class MyClass1 : EntityDto
        {
            public MyClass2 Value { get; set; }
        }

        public class MyClass2 : EntityDto
        {

        }
    }
}