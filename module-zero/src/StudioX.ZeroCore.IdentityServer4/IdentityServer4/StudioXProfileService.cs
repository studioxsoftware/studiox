using System.Threading.Tasks;
using StudioX.Authorization.Users;
using StudioX.Domain.Uow;
using StudioX.Runtime.Security;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace StudioX.IdentityServer4
{
    public class StudioXProfileService<TUser> : ProfileService<TUser> 
        where TUser : StudioXUser<TUser>
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public StudioXProfileService(
            UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IUnitOfWorkManager unitOfWorkManager
        ) : base(userManager, claimsFactory)
        {
            this.unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var tenantId = context.Subject.Identity.GetTenantId();
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await base.GetProfileDataAsync(context);
            }
        }

        [UnitOfWork]
        public override async Task IsActiveAsync(IsActiveContext context)
        {
            var tenantId = context.Subject.Identity.GetTenantId();
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await base.IsActiveAsync(context);
            }
        }
    }
}
