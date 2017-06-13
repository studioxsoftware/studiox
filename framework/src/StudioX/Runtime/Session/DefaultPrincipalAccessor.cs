using System.Security.Claims;
using System.Threading;
using StudioX.Dependency;

namespace StudioX.Runtime.Session
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor, ISingletonDependency
    {
        public virtual ClaimsPrincipal Principal =>
#if NET46
            Thread.CurrentPrincipal as ClaimsPrincipal;
#else
            null;
#endif

        public static DefaultPrincipalAccessor Instance => new DefaultPrincipalAccessor();
    }
}