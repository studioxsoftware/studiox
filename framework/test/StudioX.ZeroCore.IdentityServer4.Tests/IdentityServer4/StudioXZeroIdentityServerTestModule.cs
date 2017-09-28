using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero;
using StudioX.ZeroCore.SampleApp.Core;
using StudioX.ZeroCore.SampleApp.EntityFramework;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.IdentityServer4
{
    [DependsOn(typeof(StudioXZeroCoreIdentityServerEntityFrameworkCoreModule), typeof(StudioXZeroTestModule))]
    public class StudioXZeroIdentityServerTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            var services = new ServiceCollection();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddStudioXPersistedGrants<SampleAppDbContext>()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddStudioXIdentityServer<User>()
                .AddProfileService<StudioXProfileService<User>>();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroIdentityServerTestModule).GetAssembly());
        }
    }
}