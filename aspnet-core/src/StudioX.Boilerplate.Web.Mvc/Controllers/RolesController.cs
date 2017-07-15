using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.Permissions;
using StudioX.Boilerplate.Roles;
using StudioX.Boilerplate.Web.Models.Roles;

namespace StudioX.Boilerplate.Web.Controllers
{
    public class RolesController : BoilerplateControllerBase
    {
        private readonly IRoleAppService roleAppService;
        private readonly IPermissionAppService permissionAppService;

        public RolesController(IRoleAppService roleAppService, IPermissionAppService permissionAppService)
        {
            this.roleAppService = roleAppService;
            this.permissionAppService = permissionAppService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = (await roleAppService.GetAll(new PagedAndSortedResultRequestDto())).Items;
            var permissions = (await permissionAppService.GetAll()).Items;
            var model = new RoleListViewModel
            {
                Roles = roles,
                Permissions = permissions
            };

            return View(model);
        }

        public async Task<ActionResult> EditRoleModal(int roleId)
        {
            var role = await roleAppService.Get(new EntityDto(roleId));
            var permissions = (await permissionAppService.GetAll()).Items;
            var model = new EditRoleModalViewModel
            {
                Role = role,
                Permissions = permissions
            };
            return View("_EditRoleModal", model);
        }
    }
}