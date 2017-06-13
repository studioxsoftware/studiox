using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class StudioXProjectNameDbContextFactory : IDbContextFactory<AppDbContext>
    {
        public AppDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            builder.UseSqlServer("Server=localhost; Database=StudioXZeroMigrateTest; Trusted_Connection=True;");

            return new AppDbContext(builder.Options);
        }
    }
}