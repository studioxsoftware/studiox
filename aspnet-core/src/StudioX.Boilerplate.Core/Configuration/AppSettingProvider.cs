using System.Collections.Generic;
using StudioX.Configuration;

namespace StudioX.Boilerplate.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    AppSettingNames.UiTheme, "red",
                    scopes: SettingScopes.All,
                    isVisibleToClients: true
                )
            };
        }
    }
}