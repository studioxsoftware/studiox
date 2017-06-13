namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly SampleAppDbContext context;

        public InitialHostDbBuilder(SampleAppDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(context).Create();
            new DefaultLanguagesCreator(context).Create();
            new HostRoleAndUserCreator(context).Create();
            new DefaultSettingsCreator(context).Create();

            context.SaveChanges();
        }
    }
}
