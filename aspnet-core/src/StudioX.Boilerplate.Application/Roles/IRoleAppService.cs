using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleInput, UpdateRoleInput>
    {
        Task GrantAllPermissionsForHost(string password);

        Task GrantAllPermissionsForAllTenant(string password);
    }
}