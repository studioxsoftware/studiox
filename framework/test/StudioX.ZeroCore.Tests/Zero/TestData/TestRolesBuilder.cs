using System.Collections.Generic;
using System.Security.Claims;
using StudioX.Authorization.Roles;
using StudioX.ZeroCore.SampleApp.Core;
using StudioX.ZeroCore.SampleApp.EntityFramework;

namespace StudioX.Zero.TestData
{
    public class TestRolesBuilder
    {
        private readonly SampleAppDbContext context;
        private readonly int tenantId;

        public TestRolesBuilder(SampleAppDbContext context, int tenantId)
        {
            this.context = context;
            this.tenantId = tenantId;
        }

        public void Create()
        {
            var role = new Role(tenantId, "ADMIN", "ADMIN")
            {
                Claims = new List<RoleClaim>()
            };

            role.Claims.Add(new RoleClaim(role, new Claim("MyClaim1", "MyClaim1Value")));
            role.Claims.Add(new RoleClaim(role, new Claim("MyClaim2", "MyClaim2Value")));

            context.Roles.Add(role);

            context.SaveChanges();
        }
    }
}