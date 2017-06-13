using System;
using System.ComponentModel.DataAnnotations;

namespace StudioX.Zero.SampleApp.Users.Dto
{
    public class UpdateUserInput
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string EmailAddress { get; set; }

        public DateTime? LastLoginTime { get; set; }
    }
}