using System.Reflection;
using StudioX.Modules;
using StudioX.Web;

namespace StudioX.Owin
{
    /// <summary>
    /// OWIN integration module for StudioX.
    /// </summary>
    [DependsOn(typeof (StudioXWebCommonModule))]
    public class StudioXOwinModule : StudioXModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
