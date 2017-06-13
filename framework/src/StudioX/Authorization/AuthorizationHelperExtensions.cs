using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using StudioX.Threading;

namespace StudioX.Authorization
{
    public static class AuthorizationHelperExtensions
    {
        public static async Task AuthorizeAsync(this IAuthorizationHelper authorizationHelper, IStudioXAuthorizeAttribute authorizeAttribute)
        {
            await authorizationHelper.AuthorizeAsync(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, IEnumerable<IStudioXAuthorizeAttribute> authorizeAttributes)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(authorizeAttributes));
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, IStudioXAuthorizeAttribute authorizeAttribute)
        {
            authorizationHelper.Authorize(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, MethodInfo methodInfo)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(methodInfo));
        }
    }
}