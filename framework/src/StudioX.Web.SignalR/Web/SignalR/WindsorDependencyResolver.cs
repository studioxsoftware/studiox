using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;

namespace StudioX.Web.SignalR
{
    /// <summary>
    /// Replaces <see cref="DefaultDependencyResolver"/> to resolve dependencies from Castle Windsor (<see cref="IWindsorContainer"/>).
    /// </summary>
    public class WindsorDependencyResolver : DefaultDependencyResolver
    {
        private readonly IWindsorContainer windsorContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorDependencyResolver"/> class.
        /// </summary>
        /// <param name="windsorContainer">The windsor container.</param>
        public WindsorDependencyResolver(IWindsorContainer windsorContainer)
        {
            this.windsorContainer = windsorContainer;
        }
        
        public override object GetService(Type serviceType)
        {
            return windsorContainer.Kernel.HasComponent(serviceType)
                ? windsorContainer.Resolve(serviceType)
                : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return windsorContainer.Kernel.HasComponent(serviceType)
                ? windsorContainer.ResolveAll(serviceType).Cast<object>()
                : base.GetServices(serviceType);
        }
    }
}