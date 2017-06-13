using System.Reflection;
using StudioX.Modules;
using StudioX.Zero.EntityFramework;

namespace StudioX.Zero.SampleApp.EntityFramework
{
    [DependsOn(typeof(StudioXZeroEntityFrameworkModule), typeof(SampleAppModule))]
    public class SampleAppEntityFrameworkModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
