using System.Collections.Generic;
using System.Reflection;
using StudioX.EntityFramework.GraphDiff.Configuration;
using StudioX.EntityFramework.GraphDiff.Mapping;
using StudioX.Modules;

namespace StudioX.EntityFramework.GraphDiff
{
    [DependsOn(typeof(StudioXEntityFrameworkModule), typeof(StudioXKernelModule))]
    public class StudioXEntityFrameworkGraphDiffModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXEntityFrameworkGraphDiffModuleConfiguration, StudioXEntityFrameworkGraphDiffModuleConfiguration>();

            Configuration.Modules
                .StudioXEfGraphDiff()
                .UseMappings(new List<EntityMapping>());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
