using StudioX.AspNetCore.Mvc.Controllers;
using StudioX.Runtime.Session;

namespace StudioX.AspNetCore.App.Controllers
{
    public class MultiTenancyTestController : StudioXController
    {
        private readonly IStudioXSession studioXSession;

        public MultiTenancyTestController(IStudioXSession session)
        {
            studioXSession = session;
        }

        public int? GetTenantId()
        {
            return studioXSession.TenantId;
        }
    }
}
