using System.Reflection;
using StudioX.AspNetCore;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Resources.Embedded;

namespace StudioXAspNetCoreDemo.PlugIn
{
    [DependsOn(typeof(StudioXAspNetCoreModule))]
    public class StudioXAspNetCoreDemoPlugInModule : StudioXModule
    {
        public override void PreInitialize()
        {

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    typeof(StudioXAspNetCoreDemoPlugInModule).GetAssembly(),
                    "StudioXAspNetCoreDemo.PlugIn.Views"
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXAspNetCoreDemoPlugInModule).GetAssembly());
        }
    }
}
