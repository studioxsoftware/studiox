using System;
using System.Runtime.Serialization;
using StudioX.Web.Models;

namespace StudioX.WebApi.Client
{
    /// <summary>
    /// This exception is thrown when a remote method call made and remote application sent an error message.
    /// </summary>
    [Serializable]
    public class StudioXRemoteCallException : StudioXException
    {
        /// <summary>
        /// Remote error information.
        /// </summary>
        public ErrorInfo ErrorInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="StudioXException"/> object.
        /// </summary>
        public StudioXRemoteCallException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXException"/> object.
        /// </summary>
        public StudioXRemoteCallException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXException"/> object.
        /// </summary>
        /// <param name="errorInfo">Exception message</param>
        public StudioXRemoteCallException(ErrorInfo errorInfo)
            : base(errorInfo.Message)
        {
            ErrorInfo = errorInfo;
        }
    }
}
