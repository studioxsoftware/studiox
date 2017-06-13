using System.Collections.Generic;
using System.Reflection;
using StudioX.EntityFramework.GraphDiff;
using StudioX.EntityFramework.GraphDiff.Configuration;
using StudioX.EntityFramework.GraphDiff.Mapping;
using StudioX.EntityFramework.GraphDIff.Tests.Entities;
using StudioX.Modules;
using StudioX.TestBase;
using RefactorThis.GraphDiff;

namespace StudioX.EntityFramework.GraphDIff.Tests
{
    [DependsOn(typeof(StudioXEntityFrameworkGraphDiffModule), typeof(StudioXTestBaseModule))]
    public class StudioXEntityFrameworkGraphDiffTestModule : StudioXModule
    {
        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.StudioXEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<MyMainEntity>(config => config.AssociatedCollection(entity => entity.MyDependentEntities)),
                MappingExpressionBuilder.For<MyDependentEntity>(config => config.AssociatedEntity(entity => entity.MyMainEntity))
            };
        }
    }
}