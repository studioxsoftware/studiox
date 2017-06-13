using System.Reflection;
using StudioX.Modules;
using StudioX.Zero.EntityFrameworkCore;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore
{
    [DependsOn(typeof(StudioXZeroEntityFrameworkCoreModule), typeof(SampleAppModule))]
    public class SampleAppEntityFrameworkCoreModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
