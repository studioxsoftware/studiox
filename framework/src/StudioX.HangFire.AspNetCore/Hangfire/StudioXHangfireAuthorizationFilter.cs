using StudioX.Authorization;
using StudioX.Extensions;
using StudioX.Runtime.Session;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.Hangfire
{
    public class StudioXHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string requiredPermissionName;

        public StudioXHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            this.requiredPermissionName = requiredPermissionName;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn(context))
            {
                return false;
            }

            if (!requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(context, requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private static bool IsLoggedIn(DashboardContext context)
        {
            var session = context.GetHttpContext().RequestServices.GetRequiredService<IStudioXSession>();
            return session.UserId.HasValue;
        }

        private static bool IsPermissionGranted(DashboardContext context, string requiredPermissionName)
        {
            var permissionChecker = context.GetHttpContext().RequestServices.GetRequiredService<IPermissionChecker>();
            return permissionChecker.IsGranted(requiredPermissionName);
        }
    }
}
