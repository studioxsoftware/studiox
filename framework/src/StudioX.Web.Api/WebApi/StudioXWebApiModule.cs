using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using StudioX.Logging;
using StudioX.Modules;
using StudioX.Web;
using StudioX.WebApi.Configuration;
using StudioX.WebApi.Controllers;
using StudioX.WebApi.Controllers.Dynamic;
using StudioX.WebApi.Controllers.Dynamic.Formatters;
using StudioX.WebApi.Controllers.Dynamic.Selectors;
using StudioX.WebApi.Runtime.Caching;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using StudioX.Configuration.Startup;
using StudioX.Json;
using StudioX.WebApi.Auditing;
using StudioX.WebApi.Authorization;
using StudioX.WebApi.Controllers.ApiExplorer;
using StudioX.WebApi.Controllers.Dynamic.Binders;
using StudioX.WebApi.Controllers.Dynamic.Builders;
using StudioX.WebApi.ExceptionHandling;
using StudioX.WebApi.Security.AntiForgery;
using StudioX.WebApi.Uow;
using StudioX.WebApi.Validation;

namespace StudioX.WebApi
{
    /// <summary>
    /// This module provides StudioX features for ASP.NET Web API.
    /// </summary>
    [DependsOn(typeof(StudioXWebModule))]
    public class StudioXWebApiModule : StudioXModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());

            IocManager.Register<IDynamicApiControllerBuilder, DynamicApiControllerBuilder>();
            IocManager.Register<IStudioXWebApiConfiguration, StudioXWebApiConfiguration>();

            Configuration.Settings.Providers.Add<ClearCacheSettingProvider>();

            Configuration.Modules.StudioXWebApi().ResultWrappingIgnoreUrls.Add("/swagger");
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var httpConfiguration = IocManager.Resolve<IStudioXWebApiConfiguration>().HttpConfiguration;

            InitializeAspNetServices(httpConfiguration);
            InitializeFilters(httpConfiguration);
            InitializeFormatters(httpConfiguration);
            InitializeRoutes(httpConfiguration);
            InitializeModelBinders(httpConfiguration);

            foreach (var controllerInfo in IocManager.Resolve<DynamicApiControllerManager>().GetAll())
            {
                IocManager.IocContainer.Register(
                    Component.For(controllerInfo.InterceptorType).LifestyleTransient(),
                    Component.For(controllerInfo.ApiControllerType)
                        .Proxy.AdditionalInterfaces(controllerInfo.ServiceInterfaceType)
                        .Interceptors(controllerInfo.InterceptorType)
                        .LifestyleTransient()
                    );

                LogHelper.Logger.DebugFormat("Dynamic web api controller is created for type '{0}' with service name '{1}'.", controllerInfo.ServiceInterfaceType.FullName, controllerInfo.ServiceName);
            }

            Configuration.Modules.StudioXWebApi().HttpConfiguration.EnsureInitialized();
        }

        private void InitializeAspNetServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpControllerSelector), new StudioXHttpControllerSelector(httpConfiguration, IocManager.Resolve<DynamicApiControllerManager>()));
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new StudioXApiControllerActionSelector(IocManager.Resolve<IStudioXWebApiConfiguration>()));
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), new StudioXApiControllerActivator(IocManager));
            httpConfiguration.Services.Replace(typeof(IApiExplorer), IocManager.Resolve<StudioXApiExplorer>());
        }

        private void InitializeFilters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXApiAuthorizeFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXAntiForgeryApiFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXApiAuditFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXApiValidationFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXApiUowFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<StudioXApiExceptionFilterAttribute>());

            httpConfiguration.MessageHandlers.Add(IocManager.Resolve<ResultWrapperHandler>());
        }

        private static void InitializeFormatters(HttpConfiguration httpConfiguration)
        {
            //Remove formatters except JsonFormatter.
            foreach (var currentFormatter in httpConfiguration.Formatters.ToList())
            {
                if (!(currentFormatter is JsonMediaTypeFormatter || 
                    currentFormatter is JQueryMvcFormUrlEncodedFormatter))
                {
                    httpConfiguration.Formatters.Remove(currentFormatter);
                }
            }

            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Insert(0, new StudioXDateTimeConverter());
            httpConfiguration.Formatters.Add(new PlainTextFormatter());
        }

        private static void InitializeRoutes(HttpConfiguration httpConfiguration)
        {
            //Dynamic Web APIs

            httpConfiguration.Routes.MapHttpRoute(
                name: "StudioXDynamicWebApi",
                routeTemplate: "api/services/{*serviceNameWithAction}"
                );

            //Other routes

            httpConfiguration.Routes.MapHttpRoute(
                name: "StudioXCacheController_Clear",
                routeTemplate: "api/StudioXCache/Clear",
                defaults: new { controller = "StudioXCache", action = "Clear" }
                );

            httpConfiguration.Routes.MapHttpRoute(
                name: "StudioXCacheController_ClearAll",
                routeTemplate: "api/StudioXCache/ClearAll",
                defaults: new { controller = "StudioXCache", action = "ClearAll" }
                );
        }

        private static void InitializeModelBinders(HttpConfiguration httpConfiguration)
        {
            var studioxApiDateTimeBinder = new StudioXApiDateTimeBinder();
            httpConfiguration.BindParameter(typeof(DateTime), studioxApiDateTimeBinder);
            httpConfiguration.BindParameter(typeof(DateTime?), studioxApiDateTimeBinder);
        }
    }
}
