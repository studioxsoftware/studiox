using System;
using System.Runtime.Serialization;

namespace StudioX
{
    /// <summary>
    ///     Base exception type for those are thrown by StudioX system for StudioX specific exceptions.
    /// </summary>
    [Serializable]
    public class StudioXException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="StudioXException" /> object.
        /// </summary>
        public StudioXException()
        {
        }

#if NET46
/// <summary>
/// Creates a new <see cref="StudioXException"/> object.
/// </summary>
        public StudioXException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
#endif

        /// <summary>
        ///     Creates a new <see cref="StudioXException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public StudioXException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="StudioXException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public StudioXException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}