using System.Threading.Tasks;
using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using StudioX.Application.Services.Dto;
using StudioX.Threading;

namespace StudioX.Boilerplate.Web.Controllers
{
    [StudioXMvcAuthorize(PermissionNames.System.Administration.Tenants.MainMenu)]
    public class TenantsController : BoilerplateControllerBase
    {
        private readonly ITenantAppService tenantAppService;

        public TenantsController(ITenantAppService tenantAppService)
        {
            this.tenantAppService = tenantAppService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await tenantAppService.GetAll(new PagedResultRequestDto { MaxResultCount = 20, SkipCount = 0 });
            return View(output);
        }

        public async Task<ActionResult> EditTenantModal(int tenantId)
         {
             var tenantDto = await tenantAppService.Get(new EntityDto(tenantId));
            return View("_EditTenantModal", tenantDto);
         }
}
}