using System;
using StudioX.Zero.Configuration;

namespace StudioX.Zero.Ldap.Configuration
{
    public class StudioXZeroLdapModuleConfig : IStudioXZeroLdapModuleConfig
    {
        public bool IsEnabled { get; private set; }

        public Type AuthenticationSourceType { get; private set; }

        private readonly IStudioXZeroConfig zeroConfig;

        public StudioXZeroLdapModuleConfig(IStudioXZeroConfig zeroConfig)
        {
            this.zeroConfig = zeroConfig;
        }

        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;

            zeroConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}