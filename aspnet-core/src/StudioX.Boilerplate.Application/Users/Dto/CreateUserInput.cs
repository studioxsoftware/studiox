using System.ComponentModel.DataAnnotations;
using StudioX.Auditing;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    [AutoMap(typeof(User))]
    public class CreateUserInput 
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

        public string[] Roles { get; set; }

        [Required]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [Required]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string ConfirmPassword { get; set; }
    }
}