using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using StudioX.Authorization.Roles;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Roles;

namespace StudioX.Boilerplate.Roles.Dto
{
    [AutoMapTo(typeof(Role))]
    public class CreateRoleInput
    {
        [Required]
        [StringLength(StudioXRoleBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(StudioXRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }

        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsStatic { get; set; }

        public List<string> Permissions { get; set; }
    }
}