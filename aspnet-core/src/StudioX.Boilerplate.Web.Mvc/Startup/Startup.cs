using System;
using StudioX.AspNetCore;
using StudioX.Castle.Logging.Log4Net;
using StudioX.Boilerplate.Authentication.JwtBearer;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Identity;
using StudioX.Boilerplate.Web.Resources;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

#if FEATURE_SIGNALR
using Owin;
using StudioX.Owin;
using StudioX.Boilerplate.Owin;
#endif

namespace StudioX.Boilerplate.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, appConfiguration);

            services.AddScoped<IWebResourceManager, WebResourceManager>();

            //Configure StudioX and Dependency Injection
            return services.AddStudioX<BoilerplateWebMvcModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseStudioXLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStudioX(); //Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

#if FEATURE_SIGNALR
            //Integrate to OWIN
            app.UseAppBuilder(ConfigureOwinServices);
#endif

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

#if FEATURE_SIGNALR
        private static void ConfigureOwinServices(IAppBuilder app)
        {
            app.Properties["host.AppName"] = "Boilerplate";

            app.UseStudioX();

            app.MapSignalR();
        }
#endif
    }
}