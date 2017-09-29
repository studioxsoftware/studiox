using System;
using StudioX.Configuration;
using StudioX.Dependency;

namespace StudioX.Timing.Timezone
{
    /// <summary>
    /// Time zone converter class
    /// </summary>
    public class TimeZoneConverter : ITimeZoneConverter, ITransientDependency
    {
        private readonly ISettingManager settingManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingManager"></param>
        public TimeZoneConverter(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
        }

        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date, int? tenantId, long userId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var usersTimezone = settingManager.GetSettingValueForUser(TimingSettingNames.TimeZone, tenantId, userId);
            if(string.IsNullOrEmpty(usersTimezone))
            {
                return date;
            }
            
            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), usersTimezone);
        }

        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date, int tenantId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var tenantsTimezone = settingManager.GetSettingValueForTenant(TimingSettingNames.TimeZone, tenantId);
            if (string.IsNullOrEmpty(tenantsTimezone))
            {
                return date;
            }

            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), tenantsTimezone);
        }
        
        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var applicationsTimezone = settingManager.GetSettingValueForApplication(TimingSettingNames.TimeZone);
            if (string.IsNullOrEmpty(applicationsTimezone))
            {
                return date;
            }

            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), applicationsTimezone);
        }
    }
}