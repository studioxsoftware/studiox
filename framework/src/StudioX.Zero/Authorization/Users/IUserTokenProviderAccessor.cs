using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Users
{
    public interface IUserTokenProviderAccessor
    {
        IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>() 
            where TUser : StudioXUser<TUser>;
    }
}