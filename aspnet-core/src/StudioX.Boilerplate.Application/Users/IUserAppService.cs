using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Users.Dto;

namespace StudioX.Boilerplate.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<ListResultDto<UserListDto>> GetAll();

        PagedResultDto<UserListDto> PagedResult(GetUsersInput input);

        Task<UserDto> Get(long id);

        Task ProhibitPermission(ProhibitPermissionInput input);

        Task ResetUserSpecificPermissions(long id);

        Task Unlock(long id);

        Task Create(CreateUserInput input);

        Task UpdateUserPermissions(UserPermissionsInput input);

        Task Update(UpdateUserInput input);

        Task ResetPassword(ChangeUserPasswordInput input);

        Task Delete(long id);
    }
}