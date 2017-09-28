using StudioX.Dependency;
using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Users
{
    public class NullUserTokenProviderAccessor : IUserTokenProviderAccessor, ISingletonDependency
    {
        public IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>() where TUser : StudioXUser<TUser>
        {
            return null;
        }
    }
}