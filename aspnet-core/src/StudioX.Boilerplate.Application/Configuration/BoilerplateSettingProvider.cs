using System.Collections.Generic;
using StudioX.Configuration;

namespace StudioX.Boilerplate.Configuration
{
    public class BoilerplateSettingProvider : SettingProvider
    {
        public const string DefaultPageSize = "10";

        public const string RolesDefaultPageSize = "RolesDefaultPageSize";
        public const string UsersDefaultPageSize = "UsersDefaultPageSize";
        public const string AuditLogsDefaultPageSize = "AuditLogsDefaultPageSize";

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(RolesDefaultPageSize, DefaultPageSize),
                new SettingDefinition(UsersDefaultPageSize, DefaultPageSize),
                new SettingDefinition(AuditLogsDefaultPageSize, DefaultPageSize)
            };
        }
    }
}