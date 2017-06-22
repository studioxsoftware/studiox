using System.Collections.Generic;
using StudioX.Configuration;
using StudioX.Localization;

namespace StudioX.Timing
{
    public class TimingSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(TimingSettingNames.TimeZone, "UTC", L("TimeZone"),
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    isVisibleToClients: true)
            };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, StudioXConsts.LocalizationSourceName);
        }
    }
}