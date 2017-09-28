using System;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.Web.Mvc.Auditing;
using StudioX.Web.Mvc.Authorization;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Controllers;
using StudioX.Web.Mvc.ModelBinding.Binders;
using StudioX.Web.Mvc.Resources.Embedded;
using StudioX.Web.Mvc.Security.AntiForgery;
using StudioX.Web.Mvc.Uow;
using StudioX.Web.Mvc.Validation;
using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Mvc
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using StudioX.
    /// </summary>
    [DependsOn(typeof(StudioXWebModule))]
    public class StudioXWebMvcModule : StudioXModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());

            IocManager.Register<IStudioXMvcConfiguration, StudioXMvcConfiguration>();

            Configuration.ReplaceService<IStudioXAntiForgeryManager, StudioXMvcAntiForgeryManager>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
            HostingEnvironment.RegisterVirtualPathProvider(IocManager.Resolve<EmbeddedResourceVirtualPathProvider>());
        }

        /// <inheritdoc/>
        public override void PostInitialize()
        {
            GlobalFilters.Filters.Add(IocManager.Resolve<StudioXMvcAuthorizeFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<StudioXAntiForgeryMvcFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<StudioXMvcAuditFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<StudioXMvcValidationFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<StudioXMvcUowFilter>());

            var studioxMvcDateTimeBinder = new StudioXMvcDateTimeBinder();
            ModelBinders.Binders.Add(typeof(DateTime), studioxMvcDateTimeBinder);
            ModelBinders.Binders.Add(typeof(DateTime?), studioxMvcDateTimeBinder);
        }
    }
}
