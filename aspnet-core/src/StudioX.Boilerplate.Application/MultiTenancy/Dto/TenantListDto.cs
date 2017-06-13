using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;

namespace StudioX.Boilerplate.MultiTenancy.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantListDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}