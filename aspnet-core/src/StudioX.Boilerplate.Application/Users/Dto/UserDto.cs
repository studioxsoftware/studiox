using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudioX.Application.Services.Dto;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        [Required]
        [StringLength(StudioXUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxLastNameLength)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(StudioXUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }
    }
}