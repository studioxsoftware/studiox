using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;
using StudioX.Boilerplate.MultiTenancy;

namespace StudioX.Boilerplate.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}