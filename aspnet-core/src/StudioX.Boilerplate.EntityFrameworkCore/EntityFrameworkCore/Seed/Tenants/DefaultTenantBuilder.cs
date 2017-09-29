using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudioX.Boilerplate.Editions;
using StudioX.Boilerplate.MultiTenancy;

namespace StudioX.Boilerplate.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly BoilerplateDbContext context;

        public DefaultTenantBuilder(BoilerplateDbContext context)
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

            var defaultTenant = context.Tenants.IgnoreQueryFilters()
                .FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
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
