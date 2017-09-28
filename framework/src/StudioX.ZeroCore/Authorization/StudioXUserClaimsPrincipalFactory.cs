using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Runtime.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StudioX.Authorization
{
    public class StudioXUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>, ITransientDependency
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        public StudioXUserClaimsPrincipalFactory(
            StudioXUserManager<TRole, TUser> userManager,
            StudioXRoleManager<TRole, TUser> roleManager,
            IOptions<IdentityOptions> optionsAccessor
            ) : base(userManager, roleManager, optionsAccessor)
        {

        }

        [UnitOfWork]
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);

            if (user.TenantId.HasValue)
            {
                principal.Identities.First().AddClaim(new Claim(StudioXClaimTypes.TenantId,user.TenantId.ToString()));
            }

            return principal;
        }
    }
}