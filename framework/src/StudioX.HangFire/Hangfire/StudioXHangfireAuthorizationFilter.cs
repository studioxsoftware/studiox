using StudioX.Authorization;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Runtime.Session;
using Hangfire.Dashboard;

namespace StudioX.Hangfire
{
    public class StudioXHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public IIocResolver IocResolver { get; set; }

        private readonly string requiredPermissionName;

        public StudioXHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            this.requiredPermissionName = requiredPermissionName;

            IocResolver = IocManager.Instance;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn())
            {
                return false;
            }

            if (!requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private bool IsLoggedIn()
        {
            using (var session = IocResolver.ResolveAsDisposable<IStudioXSession>())
            {
                return session.Object.UserId.HasValue;
            }
        }

        private bool IsPermissionGranted(string requiredPermissionName)
        {
            using (var permissionChecker = IocResolver.ResolveAsDisposable<IPermissionChecker>())
            {
                return permissionChecker.Object.IsGranted(requiredPermissionName);
            }
        }
    }
}
