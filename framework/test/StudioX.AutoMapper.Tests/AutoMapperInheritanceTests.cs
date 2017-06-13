using AutoMapper;
using Shouldly;
using Xunit;

namespace StudioX.AutoMapper.Tests
{
    public class AutoMapperInheritanceTests
    {
        private readonly IMapper mapper;

        public AutoMapperInheritanceTests()
        {
            var config = new MapperConfiguration(configuration =>
            {
                configuration.CreateAutoAttributeMaps(typeof(MyTargetClassToMap));
                configuration.CreateAutoAttributeMaps(typeof(EntityDto));
                configuration.CreateAutoAttributeMaps(typeof(DerivedEntityDto));
            });

            mapper = config.CreateMapper();
        }

        [Fact]
        public void ShouldMapDerivedToTarget()
        {
            var derived = new MyDerivedClass { Value = "fortytwo" };
            var target = mapper.Map<MyTargetClassToMap>(derived);
            target.Value.ShouldBe("fortytwo");
        }

        public class MyBaseClass
        {
            public string Value { get; set; }
        }

        public class MyDerivedClass : MyBaseClass
        {

        }

        [AutoMapFrom(typeof(MyBaseClass))]
        public class MyTargetClassToMap
        {
            public string Value { get; set; }
        }

        //[Fact] //TODO: That's a problem but related to AutoMapper rather than StudioX.
        public void ShouldMapEntityProxyToEntityDtoAndToDrivedEntityDto()
        {
            var proxy = new EntityProxy() { Value = "42"};
            var target = mapper.Map<EntityDto>(proxy);
            var target2 = mapper.Map<DerivedEntityDto>(proxy);
            target.Value.ShouldBe("42");
            target2.Value.ShouldBe("42");
        }

        public class Entity
        {
            public string Value { get; set; }
        }
        public class DerivedEntity : Entity { }
        public class EntityProxy : DerivedEntity { }

        [AutoMapFrom(typeof(Entity))]
        public class EntityDto
        {
            public string Value { get; set; }
        }

        [AutoMapFrom(typeof(DerivedEntity))]
        public class DerivedEntityDto : EntityDto { }
    }
}
