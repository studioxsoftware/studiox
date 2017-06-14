using System.ComponentModel.DataAnnotations;
using StudioX.Auditing;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class ChangeUserPasswordInput
    {
        public long Id { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }
    }
}