using System.Linq;
using StudioX.ZeroCore.SampleApp.Core;

namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly SampleAppDbContext context;

        public DefaultTenantBuilder(SampleAppDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new Tenant(Tenant.DefaultTenantName, Tenant.DefaultTenantName);

                var defaultEdition = context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                context.Tenants.Add(defaultTenant);
                context.SaveChanges();
            }
        }
    }
}
