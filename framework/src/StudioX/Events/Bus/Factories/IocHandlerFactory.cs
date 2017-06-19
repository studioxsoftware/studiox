using System;
using StudioX.Dependency;
using StudioX.Events.Bus.Handlers;

namespace StudioX.Events.Bus.Factories
{
    /// <summary>
    ///     This <see cref="IEventHandlerFactory" /> implementation is used to get/release
    ///     handlers using Ioc.
    /// </summary>
    public class IocHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        ///     Type of the handler.
        /// </summary>
        public Type HandlerType { get; }

        private readonly IIocResolver iocResolver;

        /// <summary>
        ///     Creates a new instance of <see cref="IocHandlerFactory" /> class.
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="handlerType">Type of the handler</param>
        public IocHandlerFactory(IIocResolver iocResolver, Type handlerType)
        {
            this.iocResolver = iocResolver;
            HandlerType = handlerType;
        }

        /// <summary>
        ///     Resolves handler object from Ioc container.
        /// </summary>
        /// <returns>Resolved handler object</returns>
        public IEventHandler GetHandler()
        {
            return (IEventHandler) iocResolver.Resolve(HandlerType);
        }

        /// <summary>
        ///     Releases handler object using Ioc container.
        /// </summary>
        /// <param name="handler">Handler to be released</param>
        public void ReleaseHandler(IEventHandler handler)
        {
            iocResolver.Release(handler);
        }
    }
}