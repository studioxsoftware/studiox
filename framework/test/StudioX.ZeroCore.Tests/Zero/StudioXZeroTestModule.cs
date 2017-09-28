using System;
using StudioX.AutoMapper;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.TestBase;
using StudioX.Zero.Configuration;
using StudioX.ZeroCore.SampleApp;

namespace StudioX.Zero
{
    [DependsOn(typeof(StudioXZeroCoreSampleAppModule), typeof(StudioXTestBaseModule))]
    public class StudioXZeroTestModule : StudioXModule
    {
        public StudioXZeroTestModule(StudioXZeroCoreSampleAppModule sampleAppModule)
        {
            sampleAppModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
            Configuration.Modules.StudioXAutoMapper().UseStaticMapper = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.UnitOfWork.IsTransactional = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroTestModule).GetAssembly());
            TestServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
