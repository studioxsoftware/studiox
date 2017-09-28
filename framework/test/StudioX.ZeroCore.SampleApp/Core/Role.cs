using StudioX.Authorization.Roles;

namespace StudioX.ZeroCore.SampleApp.Core
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