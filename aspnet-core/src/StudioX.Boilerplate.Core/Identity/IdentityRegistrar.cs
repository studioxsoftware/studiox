using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Editions;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.Boilerplate.Identity
{
    public static class IdentityRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddLogging();

            services.AddStudioXIdentity<Tenant, User, Role>(options =>
                {
                    options.Cookies.ApplicationCookie.AuthenticationScheme = "StudioXZeroTemplateAuthSchema";
                    options.Cookies.ApplicationCookie.CookieName = "StudioXZeroTemplateAuth";
                })
                .AddStudioXTenantManager<TenantManager>()
                .AddStudioXUserManager<UserManager>()
                .AddStudioXRoleManager<RoleManager>()
                .AddStudioXEditionManager<EditionManager>()
                .AddStudioXUserStore<UserStore>()
                .AddStudioXRoleStore<RoleStore>()
                .AddStudioXLogInManager<LogInManager>()
                .AddStudioXSignInManager<SignInManager>()
                .AddStudioXSecurityStampValidator<SecurityStampValidator>()
                .AddStudioXUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();
        }
    }
}
