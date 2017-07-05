using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.MultiTenancy.Dto;

namespace StudioX.Boilerplate.MultiTenancy
{
    public interface ITenantAppService :
        IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantInput, UpdateTenantInput>
    {
       
    }
}