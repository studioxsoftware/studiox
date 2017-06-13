using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using StudioX.Dependency;

namespace StudioX.WebApi.Controllers
{
    /// <summary>
    /// This class is used to use IOC system to create api controllers.
    /// It's used by ASP.NET system.
    /// </summary>
    public class StudioXApiControllerActivator : IHttpControllerActivator
    {
        private readonly IIocResolver iocResolver;

        public StudioXApiControllerActivator(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controllerWrapper = iocResolver.ResolveAsDisposable<IHttpController>(controllerType);
            request.RegisterForDispose(controllerWrapper);
            return controllerWrapper.Object;
        }
    }
}