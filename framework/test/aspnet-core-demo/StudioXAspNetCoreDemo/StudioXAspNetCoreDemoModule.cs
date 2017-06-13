using System.Reflection;
using StudioX.AspNetCore;
using StudioX.AspNetCore.Configuration;
using StudioX.Castle.Logging.Log4Net;
using StudioX.EntityFrameworkCore;
using StudioX.EntityFrameworkCore.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioXAspNetCoreDemo.Core;
using StudioXAspNetCoreDemo.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace StudioXAspNetCoreDemo
{
    [DependsOn(
        typeof(StudioXAspNetCoreModule), 
        typeof(StudioXAspNetCoreDemoCoreModule),
        typeof(StudioXEntityFrameworkCoreModule),
        typeof(StudioXCastleLog4NetModule)
        )]
    public class StudioXAspNetCoreDemoModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = IocManager.Resolve<IConfigurationRoot>().GetConnectionString("Default");

            Configuration.Modules.StudioXEfCore().AddDbContext<MyDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    options.DbContextOptions.UseSqlServer(options.ExistingConnection);
                }
                else
                {
                    options.DbContextOptions.UseSqlServer(options.ConnectionString);
                }
            });

            Configuration.Modules.StudioXAspNetCore()
                .CreateControllersForAppServices(
                    typeof(StudioXAspNetCoreDemoCoreModule).GetAssembly()
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXAspNetCoreDemoModule).GetAssembly());
        }
    }
}