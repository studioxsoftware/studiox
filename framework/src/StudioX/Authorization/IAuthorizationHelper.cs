using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace StudioX.Authorization
{
    public interface IAuthorizationHelper
    {
        Task AuthorizeAsync(IEnumerable<IStudioXAuthorizeAttribute> authorizeAttributes);

        Task AuthorizeAsync(MethodInfo methodInfo);
    }
}