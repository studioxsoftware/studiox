using System.Threading.Tasks;
using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.Users;
using Microsoft.AspNetCore.Mvc;
using StudioX.Application.Services.Dto;

namespace StudioX.Boilerplate.Web.Controllers
{
    [StudioXMvcAuthorize(PermissionNames.System.Administration.Users.MainMenu)]
    public class UsersController : BoilerplateControllerBase
    {
        private readonly IUserAppService userAppService;

        public UsersController(IUserAppService userAppService)
        {
            this.userAppService = userAppService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = 20, SkipCount = 0 });
            return View(output);
        }
    }
}