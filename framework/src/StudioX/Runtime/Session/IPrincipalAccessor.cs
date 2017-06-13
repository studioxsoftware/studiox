using System.Security.Claims;

namespace StudioX.Runtime.Session
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
