using StudioX.Application.Services.Dto;
using StudioX.Auditing;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.UserProfile.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserProfileInfoDto : EntityDto<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        [DisableAuditing]
        public string Password { get; set; }
    }
}