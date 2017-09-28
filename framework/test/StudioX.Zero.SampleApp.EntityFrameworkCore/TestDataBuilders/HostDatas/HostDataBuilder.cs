namespace StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas
{
    public class HostDataBuilder
    {
        private readonly AppDbContext context;

        public HostDataBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            new HostUserBuilder(context).Build();
            new HostTenantsBuilder(context).Build();
        }
    }
}