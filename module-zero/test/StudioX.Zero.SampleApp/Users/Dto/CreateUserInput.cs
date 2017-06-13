using System.ComponentModel.DataAnnotations;

namespace StudioX.Zero.SampleApp.Users.Dto
{
    public class CreateUserInput
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string EmailAddress { get; set; }
    }
}