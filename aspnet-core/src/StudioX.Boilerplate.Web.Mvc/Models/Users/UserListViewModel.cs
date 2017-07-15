using System.Collections.Generic;
using StudioX.Boilerplate.Roles.Dto;
using StudioX.Boilerplate.Users.Dto;

namespace StudioX.Boilerplate.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}