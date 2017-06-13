using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Domain.Uow;
using StudioX.Runtime.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StudioX.IdentityServer4
{
    public class StudioXIdentityServerUserClaimsFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        public StudioXIdentityServerUserClaimsFactory(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
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
                principal.Identities.First().AddClaim(new Claim(StudioXClaimTypes.TenantId, user.TenantId.ToString()));
            }

            return principal;
        }
    }
}
