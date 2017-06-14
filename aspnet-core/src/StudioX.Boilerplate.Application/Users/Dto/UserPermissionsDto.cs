using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserPermissionsDto
    {
        [Required]
        public virtual long Id { get; set; }

        public virtual ICollection<UserPermissionSetting> Permissions { get; set; }
    }
}