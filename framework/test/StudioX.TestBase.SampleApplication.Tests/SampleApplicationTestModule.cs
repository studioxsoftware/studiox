using System.Reflection;
using StudioX.AutoMapper;
using StudioX.Modules;
using StudioX.TestBase.SampleApplication.Tests.Uow;

namespace StudioX.TestBase.SampleApplication.Tests
{
    [DependsOn(typeof(SampleApplicationModule), typeof(StudioXTestBaseModule), typeof(StudioXAutoMapperModule))]
    public class SampleApplicationTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.StudioXAutoMapper().UseStaticMapper = false;

            Configuration.UnitOfWork.ConventionalUowSelectors.Add(type => type == typeof(MyCustomUowClass));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
