using System;
using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.Zero.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class StudioXZeroServiceCollectionExtensions
    {
        public static StudioXIdentityBuilder AddStudioXIdentity<TTenant, TUser, TRole>(this IServiceCollection services)
            where TTenant : StudioXTenant<TUser>
            where TRole : StudioXRole<TUser>, new()
            where TUser : StudioXUser<TUser>
        {
            return services.AddStudioXIdentity<TTenant, TUser, TRole>(setupAction: null);
        }

        public static StudioXIdentityBuilder AddStudioXIdentity<TTenant, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction)
            where TTenant : StudioXTenant<TUser>
            where TRole : StudioXRole<TUser>, new()
            where TUser : StudioXUser<TUser>
        {
            services.AddSingleton<IStudioXZeroEntityTypes>(new StudioXZeroEntityTypes
            {
                Tenant = typeof(TTenant),
                Role = typeof(TRole),
                User = typeof(TUser)
            });

            //StudioXTenantManager
            services.TryAddScoped<StudioXTenantManager<TTenant, TUser>>();

            //StudioXEditionManager
            services.TryAddScoped<StudioXEditionManager>();

            //StudioXRoleManager
            services.TryAddScoped<StudioXRoleManager<TRole, TUser>>();
            services.TryAddScoped(typeof(RoleManager<TRole>), provider => provider.GetService(typeof(StudioXRoleManager<TRole, TUser>)));

            //StudioXUserManager
            services.TryAddScoped<StudioXUserManager<TRole, TUser>>();
            services.TryAddScoped(typeof(UserManager<TUser>), provider => provider.GetService(typeof(StudioXUserManager<TRole, TUser>)));

            //SignInManager
            services.TryAddScoped<StudioXSignInManager<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SignInManager<TUser>), provider => provider.GetService(typeof(StudioXSignInManager<TTenant, TRole, TUser>)));

            //StudioXLogInManager
            services.TryAddScoped<StudioXLogInManager<TTenant, TRole, TUser>>();

            //StudioXUserClaimsPrincipalFactory
            services.TryAddScoped<StudioXUserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped(typeof(UserClaimsPrincipalFactory<TUser, TRole>), provider => provider.GetService(typeof(StudioXUserClaimsPrincipalFactory<TUser, TRole>)));
            services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<TUser>), provider => provider.GetService(typeof(StudioXUserClaimsPrincipalFactory<TUser, TRole>)));

            //StudioXSecurityStampValidator
            services.TryAddScoped<StudioXSecurityStampValidator<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SecurityStampValidator<TUser>), provider => provider.GetService(typeof(StudioXSecurityStampValidator<TTenant, TRole, TUser>)));
            services.TryAddScoped(typeof(ISecurityStampValidator), provider => provider.GetService(typeof(StudioXSecurityStampValidator<TTenant, TRole, TUser>)));

            //PermissionChecker
            services.TryAddScoped<PermissionChecker<TRole, TUser>>();
            services.TryAddScoped(typeof(IPermissionChecker), provider => provider.GetService(typeof(PermissionChecker<TRole, TUser>)));

            //StudioXUserStore
            services.TryAddScoped<StudioXUserStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IUserStore<TUser>), provider => provider.GetService(typeof(StudioXUserStore<TRole, TUser>)));

            //StudioXRoleStore
            services.TryAddScoped<StudioXRoleStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IRoleStore<TRole>), provider => provider.GetService(typeof(StudioXRoleStore<TRole, TUser>)));

            //StudioXFeatureValueStore
            services.TryAddScoped<StudioXFeatureValueStore<TTenant, TUser>>();
            services.TryAddScoped(typeof(IFeatureValueStore), provider => provider.GetService(typeof(StudioXFeatureValueStore<TTenant, TUser>)));

            return new StudioXIdentityBuilder(services.AddIdentity<TUser, TRole>(setupAction), typeof(TTenant));
        }
    }
}
