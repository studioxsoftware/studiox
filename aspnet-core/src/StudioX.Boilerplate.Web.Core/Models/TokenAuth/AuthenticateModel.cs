using System.ComponentModel.DataAnnotations;
using StudioX.Authorization.Users;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Models.TokenAuth
{
    public class AuthenticateModel
    {
        [Required]
        [MaxLength(StudioXUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [MaxLength(User.MaxPlainPasswordLength)]
        public string Password { get; set; }
        
        public bool RememberClient { get; set; }
    }
}
