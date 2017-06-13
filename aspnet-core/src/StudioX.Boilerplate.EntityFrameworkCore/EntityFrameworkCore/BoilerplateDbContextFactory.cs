using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace StudioX.Boilerplate.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class BoilerplateDbContextFactory : IDbContextFactory<BoilerplateDbContext>
    {
        public BoilerplateDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<BoilerplateDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            BoilerplateDbContextConfigurer.Configure(builder, configuration.GetConnectionString(BoilerplateConsts.ConnectionStringName));
            
            return new BoilerplateDbContext(builder.Options);
        }
    }
}