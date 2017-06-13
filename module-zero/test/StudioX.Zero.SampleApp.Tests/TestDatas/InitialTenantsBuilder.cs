using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    public class InitialTenantsBuilder
    {
        private readonly AppDbContext context;

        public InitialTenantsBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            CreateTenants();
        }

        private void CreateTenants()
        {
            context.Tenants.Add(new Tenant(Tenant.DefaultTenantName, Tenant.DefaultTenantName));
            context.SaveChanges();
        }
    }
}