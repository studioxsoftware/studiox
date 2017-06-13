using System;
using System.Linq.Expressions;
using StudioX.EntityFramework.GraphDiff.Mapping;
using StudioX.EntityFramework.GraphDIff.Tests.Entities;
using RefactorThis.GraphDiff;
using Shouldly;
using Xunit;

namespace StudioX.EntityFramework.GraphDIff.Tests.Mapping
{
    public class EntityMappingManagerTests : StudioXEntityFrameworkGraphDiffTestBase
    {

        private readonly IEntityMappingManager entityMappingManager;

        public EntityMappingManagerTests()
        {
            entityMappingManager = LocalIocManager.Resolve<IEntityMappingManager>();
        }

        [Fact]
        public void MappingManagerShouldBeRegisteredAutomaticallyWithTheAssembly()
        {
            entityMappingManager.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldGetMappingForEachEntity()
        {
            var mainEntityMapping = entityMappingManager.GetEntityMappingOrNull<MyMainEntity>();
            var dependentEntityMapping = entityMappingManager.GetEntityMappingOrNull<MyDependentEntity>();

            Expression<Func<IUpdateConfiguration<MyMainEntity>, object>> expectedMainExrepssion =
                config => config.AssociatedCollection(entity => entity.MyDependentEntities);
            Expression<Func<IUpdateConfiguration<MyDependentEntity>, object>> expectedDependentExpression =
                config => config.AssociatedEntity(entity => entity.MyMainEntity);

            //Mappings shouldn't be null as they are configured
            mainEntityMapping.ShouldNotBeNull();
            dependentEntityMapping.ShouldNotBeNull();

            //Assert that string representation of mappings are equal
            mainEntityMapping.ToString().ShouldBe(expectedMainExrepssion.ToString());
            dependentEntityMapping.ToString().ShouldBe(expectedDependentExpression.ToString());
        }

        [Fact]
        public void ShouldGetNullIfMappingForEntityDoesNotExist()
        {
            entityMappingManager.GetEntityMappingOrNull<MyUnmappedEntity>().ShouldBeNull();
        }
    }
}
