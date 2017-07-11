using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Permissions.Dto;

namespace StudioX.Boilerplate.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        Task<ListResultDto<PermissionDto>> GetAll();
    }
}
