using System.ComponentModel.DataAnnotations;
using StudioX.Authorization.Users;

namespace StudioX.Boilerplate.Models.TokenAuth
{
    public class ExternalAuthenticateModel
    {
        [Required]
        [MaxLength(UserLogin.MaxLoginProviderLength)]
        public string AuthProvider { get; set; }

        [Required]
        [MaxLength(UserLogin.MaxProviderKeyLength)]
        public string ProviderKey { get; set; }

        [Required]
        public string ProviderAccessCode { get; set; }
    }
}
