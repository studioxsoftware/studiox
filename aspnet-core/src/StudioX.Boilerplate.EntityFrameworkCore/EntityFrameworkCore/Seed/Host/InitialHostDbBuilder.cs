namespace StudioX.Boilerplate.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly BoilerplateDbContext context;

        public InitialHostDbBuilder(BoilerplateDbContext context)
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
