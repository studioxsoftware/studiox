using System;
using System.Collections.Generic;
using StudioX.Application.Services.Dto;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool IsActive { get; set; }

        public bool IsLockoutEnabled { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual ICollection<UserPermissionSetting> Permissions { get; set; }
    }
}