using System.Collections.Generic;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Boilerplate.Permissions.Dto;

namespace StudioX.Boilerplate.Permissions
{
    [StudioXAuthorize]
    public class PermissionAppService : BoilerplateAppServiceBase, IPermissionAppService
    {
        public ListResultDto<PermissionDto> GetAll()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return new ListResultDto<PermissionDto>(
                    ObjectMapper.Map<List<PermissionDto>>(permissions)
                );
        }
    }
}