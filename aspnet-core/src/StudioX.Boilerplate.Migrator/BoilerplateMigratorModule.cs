using StudioX.Events.Bus;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.Migrator.DependencyInjection;

namespace StudioX.Boilerplate.Migrator
{
    [DependsOn(typeof(BoilerplateEntityFrameworkModule))]
    public class BoilerplateMigratorModule : StudioXModule
    {
        private readonly IConfigurationRoot appConfiguration;

        public BoilerplateMigratorModule(BoilerplateEntityFrameworkModule boilerplateEntityFrameworkModule)
        {
            boilerplateEntityFrameworkModule.SkipDbSeed = true;

            appConfiguration = AppConfigurations.Get(
                typeof(BoilerplateMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = appConfiguration.GetConnectionString(
                BoilerplateConsts.ConnectionStringName
                );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(typeof(IEventBus), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}