using StudioX.EntityFrameworkCore.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero.EntityFrameworkCore;
using StudioX.Boilerplate.EntityFrameworkCore.Seed;

namespace StudioX.Boilerplate.EntityFrameworkCore
{
    [DependsOn(
        typeof(BoilerplateCoreModule), 
        typeof(StudioXZeroCoreEntityFrameworkCoreModule))]
    public class BoilerplateEntityFrameworkModule : StudioXModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }


        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.StudioXEfCore().AddDbContext<BoilerplateDbContext>(configuration =>
                {
                    BoilerplateDbContextConfigurer.Configure(configuration.DbContextOptions, configuration.ConnectionString);
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}