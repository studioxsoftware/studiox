using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudioX.Auditing;
using StudioX.Authorization.Users;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Validation;
using StudioX.Extensions;

namespace StudioX.Boilerplate.Authorization.Accounts.Dto
{
    public class RegisterInput : IValidatableObject
    {
        [Required]
        [StringLength(User.MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(User.MaxLastNameLength)]
        public string LastName { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(StudioXUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
            {
                if (!UserName.Equals(EmailAddress) && ValidationHelper.IsEmail(UserName))
                {
                    yield return new ValidationResult("Username cannot be an email address unless it's same with your email address !");
                }
            }
        }
    }
}
