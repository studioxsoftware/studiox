using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.IdentityServer4
{
    public static class StudioXZeroIdentityServerIdentityBuilderExtensions
    {
        public static IdentityBuilder AddStudioXIdentityServer(this IdentityBuilder builder)
        {
            builder.AddIdentityServer();

            //StudioXIdentityServerUserClaimsPrincipalFactory
            var serviceType = typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType);
            var implementationType = typeof(StudioXIdentityServerUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType);
            builder.Services.AddScoped(serviceType, implementationType);
            
            return builder;
        }
    }
}
