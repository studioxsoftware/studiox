using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task<ListResultDto<RoleListDto>> GetAll();

        PagedResultDto<RoleListDto> PagedResult(GetRolesInput input);

        Task<RoleDto> Get(int id);

        Task<RoleDto> GetDefaultRole();

        Task Create(CreateRoleInput input);

        Task Update(UpdateRoleInput input);

        Task Delete(int id);

        Task GrantAllPermissionsForHost(string password);

        Task GrantAllPermissionsForAllTenant(string password);
    }
}