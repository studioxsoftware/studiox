using System.Threading.Tasks;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Domain.Uow;
using StudioX.MultiTenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StudioX.Authorization
{
    public class StudioXSecurityStampValidator<TTenant, TRole, TUser> : SecurityStampValidator<TUser>
        where TTenant : StudioXTenant<TUser>
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        public StudioXSecurityStampValidator(
            IOptions<IdentityOptions> options,
            StudioXSignInManager<TTenant, TRole, TUser> signInManager)
            : base(
                  options, 
                  signInManager)
        {
        }

        [UnitOfWork]
        public override Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            return base.ValidateAsync(context);
        }
    }
}
