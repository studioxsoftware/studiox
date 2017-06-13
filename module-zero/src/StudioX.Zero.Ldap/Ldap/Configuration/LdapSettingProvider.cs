using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using StudioX.Configuration;
using StudioX.Localization;

namespace StudioX.Zero.Ldap.Configuration
{
    /// <summary>
    /// Defines LDAP settings.
    /// </summary>
    public class LdapSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(LdapSettingNames.IsEnabled, "false", L("LdapIsEnabled"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false),
                       new SettingDefinition(LdapSettingNames.ContextType, ContextType.Domain.ToString(), L("LdapContextType"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false),
                       new SettingDefinition(LdapSettingNames.Container, null, L("LdapContainer"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false),
                       new SettingDefinition(LdapSettingNames.Domain, null, L("LdapDomain"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false),
                       new SettingDefinition(LdapSettingNames.UserName, null, L("LdapUserName"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false),
                       new SettingDefinition(LdapSettingNames.Password, null, L("LdapPassword"), scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false)
                   };
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, StudioXZeroConsts.LocalizationSourceName);
        }
    }
}
