using System.Collections.Generic;
using StudioX.Configuration;

namespace StudioX.Boilerplate.Configuration
{
    public class BoilerplateSettingProvider : SettingProvider
    {
        public const string DefaultPageSize = "10";

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    DefaultPageSize,
                    DefaultPageSize,
                    scopes: SettingScopes.All,
                    isVisibleToClients: true
                )
            };
        }
    }
}