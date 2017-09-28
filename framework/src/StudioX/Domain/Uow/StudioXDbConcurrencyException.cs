using System;
using System.Runtime.Serialization;

namespace StudioX.Domain.Uow
{
    [Serializable]
    public class StudioXDbConcurrencyException : StudioXException
    {
        /// <summary>
        /// Creates a new <see cref="StudioXDbConcurrencyException"/> object.
        /// </summary>
        public StudioXDbConcurrencyException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXException"/> object.
        /// </summary>
        public StudioXDbConcurrencyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public StudioXDbConcurrencyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public StudioXDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}