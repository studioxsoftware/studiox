using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Authorization;
using Microsoft.AspNetCore.Identity;
using StudioX.Authorization.Users;
using StudioX.Authorization.Roles;
using StudioX.MultiTenancy;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class StudioXZeroIdentityBuilderExtensions
    {
        public static StudioXIdentityBuilder AddStudioXTenantManager<TTenantManager>(this StudioXIdentityBuilder builder)
            where TTenantManager : class
        {
            var type = typeof(TTenantManager);
            var managerType = typeof(StudioXTenantManager<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddTransient(type, provider => provider.GetService(managerType));
            builder.Services.AddTransient(managerType, type);
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXEditionManager<TEditionManager>(this StudioXIdentityBuilder builder)
            where TEditionManager : class
        {
            var type = typeof(TEditionManager);
            var managerType = typeof(StudioXEditionManager);
            builder.Services.AddTransient(type, provider => provider.GetService(managerType));
            builder.Services.AddTransient(managerType, type);
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXRoleManager<TRoleManager>(this StudioXIdentityBuilder builder)
            where TRoleManager : class
        {
            var genericType = typeof(StudioXRoleManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(genericType, services => services.GetRequiredService(managerType));
            builder.AddRoleManager<TRoleManager>();
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXUserManager<TUserManager>(this StudioXIdentityBuilder builder)
            where TUserManager : class
        {
            var genericType = typeof(StudioXUserManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(genericType, services => services.GetRequiredService(managerType));
            builder.AddUserManager<TUserManager>();
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXSignInManager<TSignInManager>(this StudioXIdentityBuilder builder)
            where TSignInManager : class
        {
            var genericType = typeof(StudioXSignInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(genericType, services => services.GetRequiredService(managerType));
            builder.AddSignInManager<TSignInManager>();
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXLogInManager<TLogInManager>(this StudioXIdentityBuilder builder)
            where TLogInManager : class
        {
            var type = typeof(TLogInManager);
            var managerType = typeof(StudioXLogInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetService(managerType));
            builder.Services.AddScoped(managerType, type);
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXUserClaimsPrincipalFactory<TUserClaimsPrincipalFactory>(this StudioXIdentityBuilder builder)
            where TUserClaimsPrincipalFactory : class
        {
            var type = typeof(TUserClaimsPrincipalFactory);
            builder.Services.AddScoped(typeof(UserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(StudioXUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXSecurityStampValidator<TSecurityStampValidator>(this StudioXIdentityBuilder builder)
            where TSecurityStampValidator : class, ISecurityStampValidator
        {
            var type = typeof(TSecurityStampValidator);
            builder.Services.AddScoped(typeof(SecurityStampValidator<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(StudioXSecurityStampValidator<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(ISecurityStampValidator), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static StudioXIdentityBuilder AddPermissionChecker<TPermissionChecker>(this StudioXIdentityBuilder builder)
            where TPermissionChecker : class
        {
            var type = typeof(TPermissionChecker);
            var checkerType = typeof(PermissionChecker<,>).MakeGenericType(builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(checkerType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IPermissionChecker), provider => provider.GetService(type));
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXUserStore<TUserStore>(this StudioXIdentityBuilder builder)
            where TUserStore : class
        {
            var type = typeof(TUserStore);
            var genericType = typeof(StudioXUserStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IUserStore<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(genericType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static StudioXIdentityBuilder AddStudioXRoleStore<TRoleStore>(this StudioXIdentityBuilder builder)
            where TRoleStore : class
        {
            var type = typeof(TRoleStore);
            var genericType = typeof(StudioXRoleStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IRoleStore<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(genericType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static StudioXIdentityBuilder AddFeatureValueStore<TFeatureValueStore>(this StudioXIdentityBuilder builder)
            where TFeatureValueStore : class
        {
            var type = typeof(TFeatureValueStore);
            var storeType = typeof(StudioXFeatureValueStore<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(storeType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IFeatureValueStore), provider => provider.GetService(type));
            return builder;
        }
    }
}
