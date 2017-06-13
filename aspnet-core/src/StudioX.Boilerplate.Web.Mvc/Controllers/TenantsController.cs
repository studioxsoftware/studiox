using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

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

        public ActionResult Index()
        {
            var output = tenantAppService.GetTenants();
            return View(output);
        }
    }
}