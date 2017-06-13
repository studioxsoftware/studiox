using System;
using StudioX.AutoMapper;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Configuration.Startup;
using StudioX.Net.Mail;
using StudioX.TestBase;
using StudioX.Zero.Configuration;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.Tests.DependencyInjection;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace StudioX.Boilerplate.Tests
{
    [DependsOn(
        typeof(BoilerplateApplicationModule),
        typeof(BoilerplateEntityFrameworkModule),
        typeof(StudioXTestBaseModule)
        )]
    public class BoilerplateTestModule : StudioXModule
    {
        public BoilerplateTestModule(BoilerplateEntityFrameworkModule boilerplateEntityFrameworkModule)
        {
            boilerplateEntityFrameworkModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);

            //Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.StudioXAutoMapper().UseStaticMapper = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<StudioXZeroDbMigrator>();

            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
        }

        private void RegisterFakeService<TService>() where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }
    }
}