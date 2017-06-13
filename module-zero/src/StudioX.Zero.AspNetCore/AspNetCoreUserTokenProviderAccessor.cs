using StudioX.Authorization.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.DataProtection;

namespace StudioX.Zero.AspNetCore
{
    public class AspNetCoreUserTokenProviderAccessor : IUserTokenProviderAccessor
    {
        private readonly IDataProtectionProvider dataProtectionProvider;

        public AspNetCoreUserTokenProviderAccessor(IDataProtectionProvider dataProtectionProvider)
        {
            this.dataProtectionProvider = dataProtectionProvider;
        }

        public IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>()
            where TUser : StudioXUser<TUser>
        {
            return new DataProtectorUserTokenProvider<TUser>(dataProtectionProvider.CreateProtector("ASP.NET Identity"));
        }
    }
}