using StudioX.Configuration.Startup;

namespace StudioX.Zero.Ldap.Configuration
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroLdapConfigurationExtensions
    {
        /// <summary>
        /// Configures StudioX Zero LDAP module.
        /// </summary>
        /// <returns></returns>
        public static IStudioXZeroLdapModuleConfig ZeroLdap(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.StudioXConfiguration.Get<IStudioXZeroLdapModuleConfig>();
        }
    }
}
