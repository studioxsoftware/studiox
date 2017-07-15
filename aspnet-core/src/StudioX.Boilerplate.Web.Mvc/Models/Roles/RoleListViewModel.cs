using System.Collections.Generic;
using StudioX.Boilerplate.Permissions.Dto;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }
 
        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}