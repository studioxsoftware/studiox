using System;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public class StudioXIdentityBuilder : IdentityBuilder
    {
        public Type TenantType { get; }

        public StudioXIdentityBuilder(IdentityBuilder identityBuilder, Type tenantType)
            : base(identityBuilder.UserType, identityBuilder.RoleType, identityBuilder.Services)
        {
            TenantType = tenantType;
        }
    }
}