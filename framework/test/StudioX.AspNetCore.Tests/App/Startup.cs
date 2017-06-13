using System;
using StudioX.AspNetCore.TestBase;
using StudioX.Reflection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StudioX.AspNetCore.App
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var mvc = services.AddMvc();

            mvc.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(StudioXAspNetCoreModule).GetAssembly()));

            //Configure StudioX and Dependency Injection
            return services.AddStudioX<AppModule>(options =>
            {
                //Test setup
                options.SetupTest();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStudioX(); //Initializes StudioX framework.

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
