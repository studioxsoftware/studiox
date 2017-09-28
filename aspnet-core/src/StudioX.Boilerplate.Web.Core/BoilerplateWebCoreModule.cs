using System;
using System.Reflection;
using System.Text;
using StudioX.AspNetCore;
using StudioX.AspNetCore.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero.Configuration;
using StudioX.Boilerplate.Authentication.JwtBearer;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

//#if FEATURE_SIGNALR
//using StudioX.Web.SignalR;
//#endif

namespace StudioX.Boilerplate
{
    [DependsOn(
         typeof(BoilerplateApplicationModule),
         typeof(BoilerplateEntityFrameworkModule),
         typeof(StudioXAspNetCoreModule)
//#if FEATURE_SIGNALR 
//        ,typeof(StudioXWebSignalRModule)
//#endif
     )]
    public class BoilerplateWebCoreModule : StudioXModule
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfigurationRoot appConfiguration;

        public BoilerplateWebCoreModule(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
            appConfiguration = environment.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = appConfiguration.GetConnectionString(
                BoilerplateConsts.ConnectionStringName
            );

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.StudioXAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(BoilerplateApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateWebCoreModule).GetAssembly());
        }
    }
}
