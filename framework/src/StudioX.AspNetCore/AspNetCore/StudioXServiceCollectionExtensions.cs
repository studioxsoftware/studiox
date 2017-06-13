using System;
using StudioX.AspNetCore.EmbeddedResources;
using StudioX.AspNetCore.Mvc;
using StudioX.AspNetCore.Mvc.Antiforgery;
using StudioX.Dependency;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudioX.AspNetCore.Mvc.Providers;
using StudioX.Json;
using StudioX.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Options;

namespace StudioX.AspNetCore
{
    public static class StudioXServiceCollectionExtensions
    {
        /// <summary>
        /// Integrates StudioX to AspNet Core.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        public static IServiceProvider AddStudioX<TStartupModule>(this IServiceCollection services)
            where TStartupModule : StudioXModule
        {
            return services.AddStudioX<TStartupModule>(options => { });
        }

        /// <summary>
        /// Integrates StudioX to AspNet Core.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        public static IServiceProvider AddStudioX<TStartupModule>(this IServiceCollection services, Action<StudioXServiceOptions> optionsAction)
            where TStartupModule : StudioXModule
        {
            var options = new StudioXServiceOptions
            {
                IocManager = IocManager.Instance
            };

            optionsAction(options);

            ConfigureAspNetCore(services, options.IocManager);

            var bootstrapper = AddStudioXBootstrapper<TStartupModule>(services, options.IocManager);
            bootstrapper.PlugInSources.AddRange(options.PlugInSources);
            
            return WindsorRegistrationHelper.CreateServiceProvider(bootstrapper.IocManager.IocContainer, services);
        }

        private static void ConfigureAspNetCore(IServiceCollection services, IIocResolver iocResolver)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            //Use DI to create controllers
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //Use DI to create view components
            services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

            //Change anti forgery filters (to work proper with non-browser clients)
            services.Replace(ServiceDescriptor.Transient<AutoValidateAntiforgeryTokenAuthorizationFilter, StudioXAutoValidateAntiforgeryTokenAuthorizationFilter>());
            services.Replace(ServiceDescriptor.Transient<ValidateAntiforgeryTokenAuthorizationFilter, StudioXValidateAntiforgeryTokenAuthorizationFilter>());

            //Add feature providers
            var partManager = services.GetSingletonServiceOrNull<ApplicationPartManager>();
            partManager.FeatureProviders.Add(new StudioXAppServiceControllerFeatureProvider(iocResolver));

            //Configure JSON serializer
            services.Configure<MvcJsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerSettings.Converters.Insert(0, new StudioXDateTimeConverter());
            });

            //Configure MVC
            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddStudioX(services);
            });

            //Configure Razor
            services.Insert(0,
                ServiceDescriptor.Singleton<IConfigureOptions<RazorViewEngineOptions>>(
                    new ConfigureOptions<RazorViewEngineOptions>(
                        (options) =>
                        {
                            options.FileProviders.Add(new EmbeddedResourceViewFileProvider(iocResolver));
                        }
                    )
                )
            );
        }

        private static StudioXBootstrapper AddStudioXBootstrapper<TStartupModule>(IServiceCollection services, IIocManager iocManager)
            where TStartupModule : StudioXModule
        {
            var bootstrapper = StudioXBootstrapper.Create<TStartupModule>(iocManager);
            services.AddSingleton(bootstrapper);
            return bootstrapper;
        }
    }
}