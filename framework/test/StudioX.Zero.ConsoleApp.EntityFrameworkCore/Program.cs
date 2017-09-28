using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StudioX.Castle.Logging.Log4Net;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore;
using StudioX.EntityFrameworkCore.Configuration;
using StudioX.Modules;
using StudioX.MultiTenancy;
using StudioX.Zero.EntityFrameworkCore;
using StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas;
using StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas;
using StudioX.Zero.SampleApp.MultiTenancy;
using Castle.Facilities.Logging;
using Castle.LoggingFacility.MsLogging;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.ConsoleAppTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var bootstrapper = StudioXBootstrapper.Create<EfCoreTestConsoleAppModule>())
            {
                bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseStudioXLog4Net().WithConfig("log4net.config")
                );

                bootstrapper.Initialize();
                bootstrapper.IocManager.Using<MigratorRunner>(migrateTester => migrateTester.Run());
            }

            Console.WriteLine("Press Enter to EXIT!");
            Console.ReadLine();
        }
    }

    [DependsOn(typeof(SampleAppEntityFrameworkCoreModule))]
    public class EfCoreTestConsoleAppModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Server=localhost; Database=StudioXZeroMigrateTest; Trusted_Connection=True;";

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEntityFrameworkSqlServer();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var castleLoggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (castleLoggerFactory != null)
            {
                serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                    .AddCastleLogger(castleLoggerFactory);
            }

            Configuration.Modules.StudioXEfCore().AddDbContext<AppDbContext>(configuration =>
            {
                configuration.DbContextOptions
                    .UseInternalServiceProvider(serviceProvider)
                    .UseSqlServer(configuration.ConnectionString);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

    public class MigratorRunner : ITransientDependency
    {
        private readonly AppTestMigrator appTestMigrator;

        public MigratorRunner(AppTestMigrator appTestMigrator)
        {
            this.appTestMigrator = appTestMigrator;
        }

        public void Run()
        {
            List<Tenant> tenants = null;

            appTestMigrator.CreateOrMigrateForHost(context =>
            {
                new HostDataBuilder(context).Build();
                tenants = context.Tenants.ToList();
            });

            foreach (var tenant in tenants)
            {
                appTestMigrator.CreateOrMigrateForTenant(tenant, context =>
                {
                    new TenantDataBuilder(context).Build(tenant.Id);
                });
            }
        }
    }

    public class AppTestMigrator : StudioXZeroDbMigrator<AppDbContext>
    {
        public AppTestMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
            : base(unitOfWorkManager,
                  connectionStringResolver,
                  dbContextResolver)
        {

        }
    }
}
