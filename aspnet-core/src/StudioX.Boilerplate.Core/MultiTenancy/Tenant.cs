using StudioX.MultiTenancy;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.MultiTenancy
{
    public class Tenant : StudioXTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}