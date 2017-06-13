using System.Collections.Generic;
using StudioX.Configuration;
using StudioX.Localization;

namespace StudioX.Notifications
{
    public class NotificationSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    NotificationSettingNames.ReceiveNotifications,
                    "true",
                    L("ReceiveNotifications"),
                    scopes: SettingScopes.User,
                    isVisibleToClients: true)
            };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, StudioXConsts.LocalizationSourceName);
        }
    }
}
