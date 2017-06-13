using StudioX.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Authorization.Roles
{
    public class Role : StudioXRole<User>
    {
        //Can add application specific role properties here

        public Role()
        {

        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {

        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {

        }
    }
}