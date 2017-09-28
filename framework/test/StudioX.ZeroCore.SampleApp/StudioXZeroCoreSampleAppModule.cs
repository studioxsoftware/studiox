using StudioX.AutoMapper;
using StudioX.EntityFrameworkCore.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero.EntityFrameworkCore;
using StudioX.ZeroCore.SampleApp.Application;
using StudioX.ZeroCore.SampleApp.EntityFramework;
using StudioX.ZeroCore.SampleApp.EntityFramework.Seed;

namespace StudioX.ZeroCore.SampleApp
{
    [DependsOn(typeof(StudioXZeroCoreEntityFrameworkCoreModule), typeof(StudioXAutoMapperModule))]
    public class StudioXZeroCoreSampleAppModule : StudioXModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.StudioXEfCore().AddDbContext<SampleAppDbContext>(configuration =>
                {
                    StudioXZeroTemplateDbContextConfigurer.Configure(configuration.DbContextOptions, configuration.ConnectionString);
                });
            }

            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroCoreSampleAppModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
