using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using StudioX.Threading;

namespace StudioX.Authorization
{
    public static class StudioXLogInManagerExtensions
    {
        public static StudioXLoginResult<TTenant, TUser> Login<TTenant, TRole, TUser>(
            this StudioXLogInManager<TTenant, TRole, TUser> logInManager, 
            string userNameOrEmailAddress, 
            string plainPassword, 
            string tenancyName = null, 
            bool shouldLockout = true)
                where TTenant : StudioXTenant<TUser>
                where TRole : StudioXRole<TUser>, new()
                where TUser : StudioXUser<TUser>
        {
            return AsyncHelper.RunSync(
                () => logInManager.LoginAsync(
                    userNameOrEmailAddress,
                    plainPassword,
                    tenancyName,
                    shouldLockout
                )
            );
        }
    }
}
