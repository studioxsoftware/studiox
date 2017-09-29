using System.ComponentModel.DataAnnotations;

namespace StudioX.Boilerplate.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}