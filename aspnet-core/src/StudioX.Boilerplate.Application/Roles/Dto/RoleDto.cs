using System.Collections.Generic;
using StudioX.Application.Services.Dto;
using StudioX.Authorization.Roles;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Roles;

namespace StudioX.Boilerplate.Roles.Dto
{
    [AutoMapFrom(typeof(Role))]
    public class RoleDto : CreationAuditedEntityDto
    {
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Display name of this role.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Is this a static role?
        /// Static roles can not be deleted, can not change their name.
        /// They can be used programmatically.
        /// </summary>
        public virtual bool IsStatic { get; set; }

        /// <summary>
        /// Is this role will be assigned to new users as default?
        /// </summary>
        public virtual bool IsDefault { get; set; }

        public virtual ICollection<RolePermissionSetting> Permissions { get; set; }
    }
}