using System.Collections.Generic;
using System.Reflection;
using StudioX.Domain.Uow;
using StudioX.Modules;
using StudioX.Runtime.Session;
using StudioX.TestBase;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.Tests
{
    [DependsOn(
        typeof(SampleAppEntityFrameworkCoreModule),
        typeof(StudioXTestBaseModule))]
    public class SampleAppTestModule : StudioXModule
    {
        private DbContextOptions<AppDbContext> hostDbContextOptions;
        private Dictionary<int, DbContextOptions<AppDbContext>> tenantDbContextOptions;
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            SetupInMemoryDb();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void SetupInMemoryDb()
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var hostDbContextOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            hostDbContextOptionsBuilder.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider);

            hostDbContextOptions = hostDbContextOptionsBuilder.Options;
            tenantDbContextOptions = new Dictionary<int, DbContextOptions<AppDbContext>>();

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<AppDbContext>>()
                    .UsingFactoryMethod((kernel) =>
                    {
                        lock (tenantDbContextOptions)
                        {
                            var currentUow = kernel.Resolve<ICurrentUnitOfWorkProvider>().Current;
                            var session = kernel.Resolve<IStudioXSession>();

                            var tenantId = currentUow != null ? currentUow.GetTenantId() : session.TenantId;

                            if (tenantId == null)
                            {
                                return hostDbContextOptions;
                            }

                            if (!tenantDbContextOptions.ContainsKey(tenantId.Value))
                            {
                                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                                optionsBuilder.UseInMemoryDatabase(tenantId.Value.ToString()).UseInternalServiceProvider(serviceProvider);
                                tenantDbContextOptions[tenantId.Value] = optionsBuilder.Options;
                            }

                            return tenantDbContextOptions[tenantId.Value];
                        }
                    }, true)
                    .LifestyleTransient()
            );
        }
    }
}