using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Users.Dto;

namespace StudioX.Boilerplate.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(ProhibitPermissionInput input);

        Task RemoveFromRole(long userId, string roleName);

        Task<ListResultDto<UserListDto>> GetUsers();

        Task CreateUser(CreateUserInput input);
    }
}