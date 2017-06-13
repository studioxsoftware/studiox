using System.Collections.Generic;
using System.Reflection;
using System.Web;
using StudioX.Auditing;
using StudioX.Modules;
using StudioX.Runtime.Session;
using StudioX.Web.Session;
using StudioX.Configuration.Startup;
using StudioX.Web.Configuration;
using StudioX.Web.Security.AntiForgery;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Web.MultiTenancy;

namespace StudioX.Web
{
    /// <summary>
    /// This module is used to use StudioX in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(StudioXWebCommonModule))]    
    public class StudioXWebModule : StudioXModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXAntiForgeryWebConfiguration, StudioXAntiForgeryWebConfiguration>();
            IocManager.Register<IStudioXWebLocalizationConfiguration, StudioXWebLocalizationConfiguration>();
            IocManager.Register<IStudioXWebModuleConfiguration, StudioXWebModuleConfiguration>();
            
            Configuration.ReplaceService<IPrincipalAccessor, HttpContextPrincipalAccessor>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IClientInfoProvider, WebClientInfoProvider>(DependencyLifeStyle.Transient);

            Configuration.MultiTenancy.Resolvers.Add<DomainTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpHeaderTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpCookieTenantResolveContributor>();

            AddIgnoredTypes();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }

        private void AddIgnoredTypes()
        {
            var ignoredTypes = new[]
            {
                typeof(HttpPostedFileBase),
                typeof(IEnumerable<HttpPostedFileBase>),
                typeof(HttpPostedFileWrapper),
                typeof(IEnumerable<HttpPostedFileWrapper>)
            };
            
            foreach (var ignoredType in ignoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }
    }
}
