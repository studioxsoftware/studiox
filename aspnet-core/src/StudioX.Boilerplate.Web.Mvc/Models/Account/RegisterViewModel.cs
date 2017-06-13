using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using StudioX.Authorization.Users;
using StudioX.Extensions;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Web.Models.Account
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        [StringLength(StudioXUserBase.MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxLastNameLength)]
        public string LastName { get; set; }

        [StringLength(StudioXUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(StudioXUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(StudioXUserBase.MaxPlainPasswordLength)]
        public string Password { get; set; }

        public bool IsExternalLogin { get; set; }

        public string ExternalLoginAuthSchema { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
            {
                var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
                if (!UserName.Equals(EmailAddress) && emailRegex.IsMatch(UserName))
                {
                    yield return new ValidationResult("Username cannot be an email address unless it's same with your email address !");
                }
            }
        }
    }
}