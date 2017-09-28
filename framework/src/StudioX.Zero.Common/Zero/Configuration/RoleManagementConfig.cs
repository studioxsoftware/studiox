using System.Collections.Generic;

namespace StudioX.Zero.Configuration
{
    internal class RoleManagementConfig : IRoleManagementConfig
    {
        public List<StaticRoleDefinition> StaticRoles { get; private set; }

        public RoleManagementConfig()
        {
            StaticRoles = new List<StaticRoleDefinition>();
        }
    }
}