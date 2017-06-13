using System.Security.Claims;
using StudioX.MultiTenancy;

namespace StudioX.Authorization.Users
{
    public class StudioXLoginResult<TTenant, TUser>
        where TTenant : StudioXTenant<TUser>
        where TUser : StudioXUserBase
    {
        public StudioXLoginResultType Result { get; private set; }

        public TTenant Tenant { get; private set; }

        public TUser User { get; private set; }

        public ClaimsIdentity Identity { get; private set; }

        public StudioXLoginResult(StudioXLoginResultType result, TTenant tenant = null, TUser user = null)
        {
            Result = result;
            Tenant = tenant;
            User = user;
        }

        public StudioXLoginResult(TTenant tenant, TUser user, ClaimsIdentity identity)
            : this(StudioXLoginResultType.Success, tenant)
        {
            User = user;
            Identity = identity;
        }
    }
}