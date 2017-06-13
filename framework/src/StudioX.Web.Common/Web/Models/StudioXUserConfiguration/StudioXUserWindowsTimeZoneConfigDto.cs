namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXUserWindowsTimeZoneConfigDto
    {
        public string TimeZoneId { get; set; }

        public double BaseUtcOffsetInMilliseconds { get; set; }

        public double CurrentUtcOffsetInMilliseconds { get; set; }

        public bool IsDaylightSavingTimeNow { get; set; }
    }
}