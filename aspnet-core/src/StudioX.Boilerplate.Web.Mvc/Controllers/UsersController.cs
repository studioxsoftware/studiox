using System.Threading.Tasks;
using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.Users;
using Microsoft.AspNetCore.Mvc;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Roles;
using StudioX.Boilerplate.Web.Models.Users;

namespace StudioX.Boilerplate.Web.Controllers
{
    [StudioXMvcAuthorize(PermissionNames.System.Administration.Users.MainMenu)]
    public class UsersController : BoilerplateControllerBase
    {
        private readonly IUserAppService userAppService;
        private readonly IRoleAppService roleAppService;

        public UsersController(IUserAppService userAppService, IRoleAppService roleAppService)
        {
            this.userAppService = userAppService;
            this.roleAppService = roleAppService;
        }

        public async Task<ActionResult> Index()
        {
            var users = (await userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items; //Paging not implemented yet
            var roles = (await roleAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items;

            var model = new UserListViewModel
            {
                Users = users,
                Roles = roles
            };
            return View(model);
        }

        public async Task<ActionResult> EditUserModal(long userId)
        {
            var user = await userAppService.Get(new EntityDto<long>(userId));
            var roles = (await roleAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items;
            var model = new EditUserModalViewModel
            {
                User = user,
                Roles = roles
            };
            return View("_EditUserModal", model);
        }
    }
}
