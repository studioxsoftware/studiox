using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Users.Dto;

namespace StudioX.Boilerplate.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserInput, UpdateUserInput>
    {
        Task ProhibitPermission(ProhibitPermissionInput input);

        Task ResetUserSpecificPermissions(long id);

        Task Unlock(long id);

        Task ResetPassword(ChangeUserPasswordInput input);
    }
}