using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Roles;

namespace StudioX.Boilerplate.Roles.Dto
{
    [AutoMapTo(typeof(Role))]
    public class CreateRoleInput
    {
        /// <summary>
        /// Display name of this role.
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Is this role will be assigned to new users as default?
        /// </summary>
        public virtual bool IsDefault { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}