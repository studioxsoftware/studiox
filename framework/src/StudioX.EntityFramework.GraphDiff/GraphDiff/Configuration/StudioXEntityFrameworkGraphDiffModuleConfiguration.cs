using System.Collections.Generic;
using StudioX.EntityFramework.GraphDiff.Mapping;

namespace StudioX.EntityFramework.GraphDiff.Configuration
{
    public class StudioXEntityFrameworkGraphDiffModuleConfiguration : IStudioXEntityFrameworkGraphDiffModuleConfiguration
    {
        public List<EntityMapping> EntityMappings { get; set; }
    }
}