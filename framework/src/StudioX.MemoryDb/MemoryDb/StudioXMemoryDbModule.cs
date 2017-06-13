using System.Reflection;
using StudioX.MemoryDb.Configuration;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.MemoryDb
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in MemoryDb.
    /// </summary>
    public class StudioXMemoryDbModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXMemoryDbModuleConfiguration, StudioXMemoryDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXMemoryDbModule).GetAssembly());
        }
    }
}
