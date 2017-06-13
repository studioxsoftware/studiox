using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Boilerplate.Roles.Dto;

namespace StudioX.Boilerplate.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
    }
}
