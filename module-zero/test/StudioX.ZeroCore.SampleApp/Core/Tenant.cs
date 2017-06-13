using StudioX.MultiTenancy;

namespace StudioX.ZeroCore.SampleApp.Core
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