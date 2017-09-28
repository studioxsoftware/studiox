using StudioX.Authorization;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Authorization
{
    public class AppPermissionChecker : PermissionChecker<Role, User>
    {
        public AppPermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
