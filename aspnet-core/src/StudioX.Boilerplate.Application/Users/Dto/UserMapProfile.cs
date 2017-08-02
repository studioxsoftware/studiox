using AutoMapper;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Users.Dto
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<UpdateUserInput, User>();
            CreateMap<UpdateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            CreateMap<CreateUserInput, User>();
            CreateMap<CreateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }
}