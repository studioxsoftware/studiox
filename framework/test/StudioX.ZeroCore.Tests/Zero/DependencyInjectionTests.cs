using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Xunit;

using SecurityStampValidator = StudioX.ZeroCore.SampleApp.Core.SecurityStampValidator;

namespace StudioX.Zero
{
    public class DependencyInjectionTests : StudioXZeroTestBase
    {
        [Fact]
        public void ShouldResolveUserManager()
        {
            LocalIocManager.Resolve<UserManager<User>>();
            LocalIocManager.Resolve<StudioXUserManager<Role, User>>();
            LocalIocManager.Resolve<UserManager>();
        }

        [Fact]
        public void ShouldResolveRoleManager()
        {
            LocalIocManager.Resolve<RoleManager<Role>>();
            LocalIocManager.Resolve<StudioXRoleManager<Role, User>>();
            LocalIocManager.Resolve<RoleManager>();
        }

        [Fact]
        public void ShouldResolveSignInManager()
        {
            LocalIocManager.Resolve<SignInManager<User>>();
            LocalIocManager.Resolve<StudioXSignInManager<Tenant, Role, User>>();
            LocalIocManager.Resolve<SignInManager>();
        }

        [Fact]
        public void ShouldResolveLoginManager()
        {
            LocalIocManager.Resolve<StudioXLogInManager<Tenant, Role, User>>();
            LocalIocManager.Resolve<LogInManager>();
        }

        [Fact]
        public void ShouldResolveSecurityStampValidator()
        {
            LocalIocManager.Resolve<StudioXSecurityStampValidator<Tenant, Role, User>>();
            LocalIocManager.Resolve<SecurityStampValidator<User>>();
            LocalIocManager.Resolve<SecurityStampValidator>();
        }

        [Fact]
        public void ShouldResolveUserClaimsPrincipalFactory()
        {
            LocalIocManager.Resolve<UserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<StudioXUserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<IUserClaimsPrincipalFactory<User>>();
            LocalIocManager.Resolve<UserClaimsPrincipalFactory>();
        }

        [Fact]
        public void ShouldResolveTenantManager()
        {
            LocalIocManager.Resolve<StudioXTenantManager<Tenant, User>>();
            LocalIocManager.Resolve<TenantManager>();
        }

        [Fact]
        public void ShouldResolveEditionManager()
        {
            LocalIocManager.Resolve<StudioXEditionManager>();
            LocalIocManager.Resolve<EditionManager>();
        }

        [Fact]
        public void ShouldResolvePermissionChecker()
        {
            LocalIocManager.Resolve<IPermissionChecker>();
            LocalIocManager.Resolve<PermissionChecker<Role, User>>();
            LocalIocManager.Resolve<PermissionChecker>();
        }

        [Fact]
        public void ShouldResolveFeatureValueStore()
        {
            LocalIocManager.Resolve<IFeatureValueStore>();
            LocalIocManager.Resolve<StudioXFeatureValueStore<Tenant, User>>();
            LocalIocManager.Resolve<FeatureValueStore>();
        }

        [Fact]
        public void ShouldResolveUserStore()
        {
            LocalIocManager.Resolve<IUserStore<User>>();
            LocalIocManager.Resolve<StudioXUserStore<Role, User>>();
            LocalIocManager.Resolve<UserStore>();
        }

        [Fact]
        public void ShouldResolveRoleStore()
        {
            LocalIocManager.Resolve<IRoleStore<Role>>();
            LocalIocManager.Resolve<StudioXRoleStore<Role, User>>();
            LocalIocManager.Resolve<RoleStore>();
        }
    }
}
