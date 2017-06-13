using System.Reflection;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.TestBase;

namespace StudioX.Web.Common.Tests
{
    [DependsOn(typeof(StudioXWebCommonModule), typeof(StudioXTestBaseModule))]
    public class StudioXWebCommonTestModule : StudioXModule
    {
        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(typeof(StudioXWebCommonTestModule).GetAssembly());
        }
    }
}
