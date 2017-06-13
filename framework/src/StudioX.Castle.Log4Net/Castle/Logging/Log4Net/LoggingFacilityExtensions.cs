using Castle.Facilities.Logging;

namespace StudioX.Castle.Logging.Log4Net
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseStudioXLog4Net(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<Log4NetLoggerFactory>();
        }
    }
}