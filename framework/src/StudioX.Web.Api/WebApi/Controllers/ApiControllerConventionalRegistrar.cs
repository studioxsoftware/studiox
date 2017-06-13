using System.Web.Http;
using StudioX.Dependency;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace StudioX.WebApi.Controllers
{
    /// <summary>
    /// Registers all Web API Controllers derived from <see cref="ApiController"/>.
    /// </summary>
    public class ApiControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<ApiController>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .LifestyleTransient()
                );
        }
    }
}
