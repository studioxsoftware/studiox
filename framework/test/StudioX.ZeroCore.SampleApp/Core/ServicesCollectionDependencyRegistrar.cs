using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.ZeroCore.SampleApp.Core
{
    public static class ServicesCollectionDependencyRegistrar
    {
        public static void Register(ServiceCollection services)
        {
            services.AddLogging();

            services.AddStudioXIdentity<Tenant, User, Role>()
                .AddStudioXTenantManager<TenantManager>()
                .AddStudioXEditionManager<EditionManager>()
                .AddStudioXRoleManager<RoleManager>()
                .AddStudioXUserManager<UserManager>()
                .AddStudioXSignInManager<SignInManager>()
                .AddStudioXLogInManager<LogInManager>()
                .AddStudioXUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddStudioXSecurityStampValidator<SecurityStampValidator>()
                .AddPermissionChecker<PermissionChecker>()
                .AddStudioXUserStore<UserStore>()
                .AddStudioXRoleStore<RoleStore>()
                .AddFeatureValueStore<FeatureValueStore>()
                .AddDefaultTokenProviders();
        }
    }
}
