using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudioX.Boilerplate.Users.Dto
{
    public class UserPermissionsInput
    {
        /// <summary>
        /// Display name of this role.
        /// </summary>
        [Required]
        public virtual long Id { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}