using System.Linq;
using StudioX.Authorization.Users;
using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    public class InitialUserOrganizationUnitsBuilder
    {
        private readonly AppDbContext context;

        public InitialUserOrganizationUnitsBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            AddUsersToOus();
        }

        private void AddUsersToOus()
        {
            var defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);
            var adminUser = context.Users.Single(u => u.TenantId == defaultTenant.Id && u.UserName == User.AdminUserName);

            var ou11 = context.OrganizationUnits.Single(ou => ou.DisplayName == "OU11");
            var ou21 = context.OrganizationUnits.Single(ou => ou.DisplayName == "OU21");

            context.UserOrganizationUnits.Add(new UserOrganizationUnit(defaultTenant.Id, adminUser.Id, ou11.Id));
            context.UserOrganizationUnits.Add(new UserOrganizationUnit(defaultTenant.Id, adminUser.Id, ou21.Id));
        }
    }
}