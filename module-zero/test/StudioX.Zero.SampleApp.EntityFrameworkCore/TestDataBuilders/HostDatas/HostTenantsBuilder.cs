using System;
using System.Linq;
using StudioX.Extensions;
using StudioX.Runtime.Security;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas
{
    public class HostTenantsBuilder
    {
        private readonly AppDbContext context;

        public HostTenantsBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            CreateTenants();
        }

        private void CreateTenants()
        {
            CreateTenantIfNotExists(MultiTenancy.Tenant.DefaultTenantName);
        }

        private void CreateTenantIfNotExists(string tenancyName)
        {
            if (context.Tenants.Any(t => t.TenancyName == tenancyName))
            {
                return;
            }

            var tenant = context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName);
            if (tenant == null)
            {
                tenant = context.Tenants.Add(
                    new MultiTenancy.Tenant(tenancyName, tenancyName)
                    {
                        ConnectionString = SimpleStringCipher.Instance.Encrypt(
                            $"server=localhost;database=StudioXZeroTenantDb_{tenancyName}_{Guid.NewGuid().ToString("N").Left(8)};trusted_connection=true;"
                        )
                    }).Entity;

                context.SaveChanges();
            }
        }
    }
}