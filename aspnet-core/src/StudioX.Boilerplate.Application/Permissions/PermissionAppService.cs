using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Boilerplate.Permissions.Dto;

namespace StudioX.Boilerplate.Permissions
{
    [StudioXAuthorize]
    public class PermissionAppService : BoilerplateAppServiceBase, IPermissionAppService
    {
        public Task<ListResultDto<PermissionDto>> GetAll()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                    ObjectMapper.Map<List<PermissionDto>>(permissions)
                ));
        }
    }
}