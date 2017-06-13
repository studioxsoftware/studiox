using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Users;

namespace StudioX.Boilerplate.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }
    }
}
