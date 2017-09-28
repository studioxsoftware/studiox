using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using StudioX.Domain.Entities;
using StudioX.Domain.Entities.Auditing;

namespace StudioX.Authorization.Users
{
    [Table("UserClaims")]
    public class UserClaim : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        public virtual long UserId { get; set; }

        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public UserClaim()
        {
            
        }

        public UserClaim(StudioXUserBase user, Claim claim)
        {
            TenantId = user.TenantId;
            UserId = user.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
