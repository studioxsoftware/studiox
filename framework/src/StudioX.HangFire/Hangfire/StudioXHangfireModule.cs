using StudioX.Hangfire.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using Hangfire;

namespace StudioX.Hangfire
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXHangfireModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXHangfireConfiguration, StudioXHangfireConfiguration>();
            
            Configuration.Modules
                .StudioXHangfire()
                .GlobalConfiguration
                .UseActivator(new HangfireIocJobActivator(IocManager));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXHangfireModule).GetAssembly());
            GlobalJobFilters.Filters.Add(IocManager.Resolve<StudioXHangfireJobExceptionFilter>());
        }
    }
}
