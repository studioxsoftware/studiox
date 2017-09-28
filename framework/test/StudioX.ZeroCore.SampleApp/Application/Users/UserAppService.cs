using StudioX.Application.Services;
using StudioX.Domain.Repositories;
using StudioX.ZeroCore.SampleApp.Core;

namespace StudioX.ZeroCore.SampleApp.Application.Users
{
    public class UserAppService : AsyncCrudAppService<User, UserDto, long>, IUserAppService
    {
        public UserAppService(IRepository<User, long> repository) 
            : base(repository)
        {
            
        }
    }
}