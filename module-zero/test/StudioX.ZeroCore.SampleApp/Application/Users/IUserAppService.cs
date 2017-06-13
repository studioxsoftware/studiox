using StudioX.Application.Services;

namespace StudioX.ZeroCore.SampleApp.Application.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long>
    {
        
    }
}
