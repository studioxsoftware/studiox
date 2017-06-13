using System.Reflection;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// This modules is used to replace StudioX's cache system with Redis server.
    /// </summary>
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXRedisCacheModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<StudioXRedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXRedisCacheModule).GetAssembly());
        }
    }
}
