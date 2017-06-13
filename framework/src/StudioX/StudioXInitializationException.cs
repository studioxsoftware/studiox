using System;
using System.Runtime.Serialization;

namespace StudioX
{
    /// <summary>
    /// This exception is thrown if a problem on StudioX initialization progress.
    /// </summary>
    [Serializable]
    public class StudioXInitializationException : StudioXException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StudioXInitializationException()
        {

        }

#if NET46
        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public StudioXInitializationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public StudioXInitializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public StudioXInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
