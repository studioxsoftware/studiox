using System.Reflection;
using StudioX.Application.Services;
using StudioX.Configuration.Startup;
using StudioX.Modules;
using StudioX.TestBase;
using StudioX.Web.Api.Tests.AppServices;
using StudioX.WebApi;

namespace StudioX.Web.Api.Tests
{
    [DependsOn(typeof(StudioXWebApiModule), typeof(StudioXTestBaseModule))]
    public class StudioXWebApiTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();

            Configuration.Localization.IsEnabled = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.StudioXWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "myapp")
                .ForMethods(builder =>
                {
                    if (builder.Method.IsDefined(typeof(MyIgnoreApiAttribute)))
                    {
                        builder.DontCreate = true;
                    }
                })
                .WithProxyScripts(false)
                .Build();
        }
    }
}