using StudioX.Authorization.Roles;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Roles
{
    public class Role : StudioXRole<User>
    {
        public Role()
        {

        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {

        }
    }
}