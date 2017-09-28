using System;

namespace StudioX.Zero.Ldap.Configuration
{
    public interface IStudioXZeroLdapModuleConfig
    {
        bool IsEnabled { get; }

        Type AuthenticationSourceType { get; }

        void Enable(Type authenticationSourceType);
    }
}