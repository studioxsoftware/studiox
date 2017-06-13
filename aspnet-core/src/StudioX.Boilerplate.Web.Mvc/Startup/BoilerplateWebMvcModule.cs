using System.Reflection;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Boilerplate.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StudioX.Boilerplate.Web.Startup
{
    [DependsOn(typeof(BoilerplateWebCoreModule))]
    public class BoilerplateWebMvcModule : StudioXModule
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot appConfiguration;

        public BoilerplateWebMvcModule(IHostingEnvironment env)
        {
            this.env = env;
            appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<BoilerplateNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateWebMvcModule).GetAssembly());
        }
    }
}