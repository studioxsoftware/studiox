using StudioX.Zero.SampleApp.EntityFramework;
using EntityFramework.DynamicFilters;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    public class InitialTestDataBuilder
    {
        private readonly AppDbContext context;

        public InitialTestDataBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            context.DisableAllFilters();

            new InitialTenantsBuilder(context).Build();
            new InitialUsersBuilder(context).Build();
            new InitialTestLanguagesBuilder(context).Build();
            new InitialTestOrganizationUnitsBuilder(context).Build();
            new InitialUserOrganizationUnitsBuilder(context).Build();
        }
    }
}