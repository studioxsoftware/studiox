using System.Reflection;
using StudioX.Modules;
using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace StudioX.Web.SignalR
{
    /// <summary>
    /// StudioX SignalR integration module.
    /// </summary>
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXWebSignalRModule : StudioXModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            GlobalHost.DependencyResolver = new WindsorDependencyResolver(IocManager.IocContainer);
            UseStudioXSignalRContractResolver();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void UseStudioXSignalRContractResolver()
        {
            var serializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    ContractResolver = new StudioXSignalRContractResolver()
                });
            
            IocManager.IocContainer.Register(
                Component.For<JsonSerializer>().UsingFactoryMethod(() => serializer)
                );
        }
    }
}
