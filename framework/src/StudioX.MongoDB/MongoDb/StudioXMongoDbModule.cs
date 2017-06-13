using System.Reflection;
using StudioX.Modules;
using StudioX.MongoDb.Configuration;

namespace StudioX.MongoDb
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in MongoDB.
    /// </summary>
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXMongoDbModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IStudioXMongoDbModuleConfiguration, StudioXMongoDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
