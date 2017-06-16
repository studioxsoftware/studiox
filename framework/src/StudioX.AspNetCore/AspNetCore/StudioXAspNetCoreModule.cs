using System.Linq;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.MultiTenancy;
using StudioX.AspNetCore.Mvc.Auditing;
using StudioX.AspNetCore.Runtime.Session;
using StudioX.AspNetCore.Security.AntiForgery;
using StudioX.Auditing;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Runtime.Session;
using StudioX.Web;
using StudioX.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using StudioX.AspNetCore.Mvc.Results.Wrapping;

namespace StudioX.AspNetCore
{
    [DependsOn(typeof(StudioXWebCommonModule))]
    public class StudioXAspNetCoreModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new StudioXAspNetCoreConventionalRegistrar());

            IocManager.Register<IStudioXAspNetCoreConfiguration, StudioXAspNetCoreConfiguration>();
            IocManager.Register<IStudioXActionResultWrapperFactory, StudioXActionResultWrapperFactory>();

            Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IStudioXAntiForgeryManager, StudioXAspNetCoreAntiForgeryManager>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IClientInfoProvider, HttpContextClientInfoProvider>(DependencyLifeStyle.Transient);

            Configuration.Modules.StudioXAspNetCore().FormBodyBindingIgnoredTypes.Add(typeof(IFormFile));

            Configuration.MultiTenancy.Resolvers.Add<DomainTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpHeaderTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpCookieTenantResolveContributor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXAspNetCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            AddApplicationParts();
            ConfigureAntiforgery();
        }

        private void AddApplicationParts()
        {
            var configuration = IocManager.Resolve<StudioXAspNetCoreConfiguration>();
            var partManager = IocManager.Resolve<ApplicationPartManager>();
            var moduleManager = IocManager.Resolve<IStudioXModuleManager>();

            var controllerAssemblies = configuration.ControllerAssemblySettings.Select(s => s.Assembly).Distinct();
            foreach (var controllerAssembly in controllerAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
            }

            var plugInAssemblies = moduleManager.Modules.Where(m => m.IsLoadedAsPlugIn).Select(m => m.Assembly).Distinct();
            foreach (var plugInAssembly in plugInAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(plugInAssembly));
            }
        }

        private void ConfigureAntiforgery()
        {
            IocManager.Using<IOptions<AntiforgeryOptions>>(optionsAccessor =>
            {
                optionsAccessor.Value.HeaderName = Configuration.Modules.StudioXWebCommon().AntiForgery.TokenHeaderName;
            });
        }
    }
}