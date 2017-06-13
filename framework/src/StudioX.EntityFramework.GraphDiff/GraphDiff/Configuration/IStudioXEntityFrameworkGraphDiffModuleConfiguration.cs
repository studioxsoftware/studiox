using System.Collections.Generic;
using StudioX.EntityFramework.GraphDiff.Mapping;

namespace StudioX.EntityFramework.GraphDiff.Configuration
{
    public interface IStudioXEntityFrameworkGraphDiffModuleConfiguration
    {
        List<EntityMapping> EntityMappings { get; set; }
    }
}