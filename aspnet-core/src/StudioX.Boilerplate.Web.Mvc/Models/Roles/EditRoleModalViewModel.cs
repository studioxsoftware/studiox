using System.Collections.Generic;
using System.Linq;
using StudioX.Boilerplate.Permissions.Dto;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Web.Models.Roles
{
    public class EditRoleModalViewModel
    {
        public RoleDto Role { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }

        public bool HasPermission(PermissionDto permission)
        {
            return Permissions != null && Role.Permissions.Any(p => p == permission.Name);
        }
    }
}
