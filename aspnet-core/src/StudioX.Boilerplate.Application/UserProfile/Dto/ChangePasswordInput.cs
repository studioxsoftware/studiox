using System.ComponentModel.DataAnnotations;
using StudioX.Auditing;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.UserProfile.Dto
{
    public class ChangePasswordInput
    {
        /// <summary>
        ///  Password of the user.
        /// </summary>
        [Required(ErrorMessage = "Password can not be null or empty!")]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }
        
        /// <summary>
        ///  New password of the user.
        /// </summary>
        [Required(ErrorMessage = "New password can not be null or empty!")]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}