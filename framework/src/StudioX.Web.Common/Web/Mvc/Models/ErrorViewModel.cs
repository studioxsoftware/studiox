using System;
using StudioX.Web.Models;

namespace StudioX.Web.Mvc.Models
{
    public class ErrorViewModel
    {
        public ErrorInfo ErrorInfo { get; set; }

        public Exception Exception { get; set; }

        public ErrorViewModel()
        {
            
        }

        public ErrorViewModel(ErrorInfo errorInfo, Exception exception = null)
        {
            ErrorInfo = errorInfo;
            Exception = exception;
        }
    }
}
