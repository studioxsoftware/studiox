using System.Collections.Generic;
using System.Reflection;
using StudioX.AutoMapper;
using StudioX.EntityFramework;
using StudioX.EntityFramework.GraphDiff;
using StudioX.EntityFramework.GraphDiff.Configuration;
using StudioX.EntityFramework.GraphDiff.Mapping;
using StudioX.Modules;
using StudioX.TestBase.SampleApplication.ContacLists;
using StudioX.TestBase.SampleApplication.People;
using RefactorThis.GraphDiff;

namespace StudioX.TestBase.SampleApplication
{
    [DependsOn(typeof(StudioXEntityFrameworkModule), typeof(StudioXAutoMapperModule), typeof(StudioXEntityFrameworkGraphDiffModule))]
    public class SampleApplicationModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Features.Providers.Add<SampleFeatureProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.StudioXEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<ContactList>(config => config.AssociatedCollection(entity => entity.People)),
                MappingExpressionBuilder.For<Person>(config => config.AssociatedEntity(entity => entity.ContactList))
            };
        }
    }
}
