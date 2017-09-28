namespace StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas
{
    public class TenantDataBuilder
    {
        private readonly AppDbContext context;

        public TenantDataBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build(int tenantId)
        {
            new TenantUserBuilder(context).Build(tenantId);
        }
    }
}