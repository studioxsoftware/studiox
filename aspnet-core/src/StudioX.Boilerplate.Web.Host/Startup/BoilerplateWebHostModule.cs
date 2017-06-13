using System.Reflection;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Boilerplate.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StudioX.Boilerplate.Web.Host.Startup
{
    [DependsOn(
       typeof(BoilerplateWebCoreModule))]
    public class BoilerplateWebHostModule: StudioXModule
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfigurationRoot appConfiguration;

        public BoilerplateWebHostModule(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
            appConfiguration = environment.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateWebHostModule).GetAssembly());
        }
    }
}
