using StudioX.AutoMapper;
using StudioX.Boilerplate.Sessions.Dto;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}