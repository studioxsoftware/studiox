using System.ComponentModel.DataAnnotations;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.UserProfile.Dto
{
    [AutoMapTo(typeof(User))]
    public class UpdateUserProfileInput 
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        [Required]
        [StringLength(User.MaxFirstNameLength)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        [Required]
        [StringLength(User.MaxLastNameLength)]
        public string LastName { get; set; }

        /// <summary>
        ///  Email address of the user.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }
    }
}