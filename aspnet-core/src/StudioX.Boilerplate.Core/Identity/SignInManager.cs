using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudioX.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.MultiTenancy;
using StudioX.Configuration;
using StudioX.Domain.Uow;

namespace StudioX.Boilerplate.Identity
{
    public class SignInManager : StudioXSignInManager<Tenant, Role, User>
    {
        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes
        ) : base(
            userManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            unitOfWorkManager,
            settingManager,
            schemes)
        {
        }
    }
}