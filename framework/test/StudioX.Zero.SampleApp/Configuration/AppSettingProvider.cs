using System.Collections.Generic;
using StudioX.Configuration;

namespace StudioX.Zero.SampleApp.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new List<SettingDefinition>
                   {
                       new SettingDefinition("Setting1", "1"),
                       new SettingDefinition("Setting2", "A")
                   };
        }
    }
}
