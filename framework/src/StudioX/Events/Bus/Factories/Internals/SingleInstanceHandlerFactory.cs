using StudioX.Events.Bus.Handlers;

namespace StudioX.Events.Bus.Factories.Internals
{
    /// <summary>
    ///     This <see cref="IEventHandlerFactory" /> implementation is used to handle events
    ///     by a single instance object.
    /// </summary>
    /// <remarks>
    ///     This class always gets the same single instance of handler.
    /// </remarks>
    internal class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        ///     The event handler instance.
        /// </summary>
        public IEventHandler HandlerInstance { get; }

        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        public void ReleaseHandler(IEventHandler handler)
        {
        }
    }
}