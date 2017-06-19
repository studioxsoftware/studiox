using System;

namespace StudioX.Events.Bus.Exceptions
{
    /// <summary>
    ///     This type of events are used to notify for exceptions handled by StudioX infrastructure.
    /// </summary>
    public class StudioXHandledExceptionData : ExceptionData
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public StudioXHandledExceptionData(Exception exception)
            : base(exception)
        {
        }
    }
}