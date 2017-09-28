using StudioX.MultiTenancy;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.MultiTenancy
{
    public class Tenant : StudioXTenant<User>
    {
        protected Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}