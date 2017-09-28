using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using StudioX.Logging;

namespace StudioX.Runtime.Validation
{
    /// <summary>
    /// This exception type is used to throws validation exceptions.
    /// </summary>
    [Serializable]
    public class StudioXValidationException : StudioXException, IHasLogSeverity
    {
        /// <summary>
        /// Detailed list of validation errors for this exception.
        /// </summary>
        public IList<ValidationResult> ValidationErrors { get; set; }

        /// <summary>
        /// Exception severity.
        /// Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StudioXValidationException()
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public StudioXValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public StudioXValidationException(string message)
            : base(message)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="validationErrors">Validation errors</param>
        public StudioXValidationException(string message, IList<ValidationResult> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public StudioXValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = LogSeverity.Warn;
        }
    }
}
