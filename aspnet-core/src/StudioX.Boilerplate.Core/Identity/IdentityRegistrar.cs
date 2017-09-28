using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Editions;
using StudioX.Boilerplate.MultiTenancy;

namespace StudioX.Boilerplate.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddStudioXIdentity<Tenant, User, Role>()
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
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}