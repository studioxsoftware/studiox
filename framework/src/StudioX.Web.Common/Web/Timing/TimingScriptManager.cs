using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using StudioX.Configuration;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Timing;
using StudioX.Timing.Timezone;

namespace StudioX.Web.Timing
{
    /// <summary>
    /// This class is used to build timing script.
    /// </summary>
    public class TimingScriptManager : ITimingScriptManager, ITransientDependency
    {
        private readonly ISettingManager settingManager;

        public TimingScriptManager(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
        }

        public async Task<string> GetScriptAsync()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");

            script.AppendLine("    studiox.clock.provider = studiox.timing." + Clock.Provider.GetType().Name.ToCamelCase() + " || studiox.timing.localClockProvider;");
            script.AppendLine("    studiox.clock.provider.supportsMultipleTimezone = " + Clock.SupportsMultipleTimezone.ToString().ToLowerInvariant() + ";");

            if (Clock.SupportsMultipleTimezone)
            {
                script.AppendLine("    studiox.timing.timeZoneInfo = " + await GetUsersTimezoneScriptsAsync());
            }

            script.Append("})();");

            return script.ToString();
        }

        private async Task<string> GetUsersTimezoneScriptsAsync()
        {
            var timezoneId = await settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            return " {" +
                   "        windows: {" +
                   "            timeZoneId: '" + timezoneId + "'," +
                   "            baseUtcOffsetInMilliseconds: '" + timezone.BaseUtcOffset.TotalMilliseconds + "'," +
                   "            currentUtcOffsetInMilliseconds: '" + timezone.GetUtcOffset(Clock.Now).TotalMilliseconds + "'," +
                   "            isDaylightSavingTimeNow: '" + timezone.IsDaylightSavingTime(Clock.Now) + "'" +
                   "        }," +
                   "        iana: {" +
                   "            timeZoneId:'" + TimezoneHelper.WindowsToIana(timezoneId) + "'" +
                   "        }," +
                   "    }";
        }
    }
}