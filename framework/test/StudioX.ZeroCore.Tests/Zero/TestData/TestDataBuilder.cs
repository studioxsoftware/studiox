using StudioX.ZeroCore.SampleApp.EntityFramework;

namespace StudioX.Zero.TestData
{
    public class TestDataBuilder
    {
        private readonly SampleAppDbContext context;
        private readonly int tenantId;

        public TestDataBuilder(SampleAppDbContext context, int tenantId)
        {
            this.context = context;
            this.tenantId = tenantId;
        }

        public void Create()
        {
            new TestRolesBuilder(context, tenantId).Create();
            new TestOrganizationUnitsBuilder(context, tenantId).Create();

            context.SaveChanges();
        }
    }
}
