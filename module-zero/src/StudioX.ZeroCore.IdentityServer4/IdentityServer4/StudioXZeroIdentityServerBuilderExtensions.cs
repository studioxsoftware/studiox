using System;
using System.IdentityModel.Tokens.Jwt;
using StudioX.Authorization.Users;
using StudioX.IdentityServer4;
using StudioX.Runtime.Security;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class StudioXZeroIdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddStudioXIdentityServer<TUser>(this IIdentityServerBuilder builder, Action<StudioXIdentityServerOptions> optionsAction = null)
            where TUser : StudioXUser<TUser>
        {
            var options = new StudioXIdentityServerOptions();
            optionsAction?.Invoke(options);

            builder.AddAspNetIdentity<TUser>();

            builder.AddProfileService<StudioXProfileService<TUser>>();
            builder.AddResourceOwnerValidator<StudioXResourceOwnerPasswordValidator<TUser>>();

            builder.Services.Replace(ServiceDescriptor.Transient<IClaimsService, StudioXClaimsService>());

            if (options.UpdateStudioXClaimTypes)
            {
                StudioXClaimTypes.UserId = JwtClaimTypes.Subject;
                StudioXClaimTypes.UserName = JwtClaimTypes.Name;
                StudioXClaimTypes.Role = JwtClaimTypes.Role;
            }

            if (options.UpdateJwtSecurityTokenHandlerDefaultInboundClaimTypeMap)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[StudioXClaimTypes.UserId] = StudioXClaimTypes.UserId;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[StudioXClaimTypes.UserName] = StudioXClaimTypes.UserName;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[StudioXClaimTypes.Role] = StudioXClaimTypes.Role;
            }

            return builder;
        }
    }
}
