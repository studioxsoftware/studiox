using System.Linq;
using StudioX.Organizations;
using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    /* Creates OU tree for default tenant as shown below:
     * 
     * - OU1
     *   - OU11
     *     - OU111
     *     - OU112
     *   - OU12
     * - OU2
     *   - OU21
     */
    public class InitialTestOrganizationUnitsBuilder
    {
        private readonly AppDbContext context;
        private Tenant defaultTenant;

        public InitialTestOrganizationUnitsBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);

            CreateOUs();
        }

        private void CreateOUs()
        {
            var ou1 = CreateOU("OU1", OrganizationUnit.CreateCode(1));
            var ou11 = CreateOU("OU11", OrganizationUnit.CreateCode(1, 1), ou1.Id);
            var ou111 = CreateOU("OU111", OrganizationUnit.CreateCode(1, 1, 1), ou11.Id);
            var ou112 = CreateOU("OU112", OrganizationUnit.CreateCode(1, 1, 2), ou11.Id);
            var ou12 = CreateOU("OU12", OrganizationUnit.CreateCode(1, 2), ou1.Id);
            var ou2 = CreateOU("OU2", OrganizationUnit.CreateCode(2));
            var ou21 = CreateOU("OU21", OrganizationUnit.CreateCode(2, 1), ou2.Id);
        }

        private OrganizationUnit CreateOU(string displayName, string code, long? parentId = null)
        {
            var ou = context.OrganizationUnits.Add(new OrganizationUnit(defaultTenant.Id, displayName, parentId) { Code = code });
            context.SaveChanges();
            return ou;
        }
    }
}
