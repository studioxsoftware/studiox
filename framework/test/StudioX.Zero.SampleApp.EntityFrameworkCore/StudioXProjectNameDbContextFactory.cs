using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class StudioXProjectNameDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            builder.UseSqlServer("Server=localhost; Database=StudioXZeroMigrateTest; Trusted_Connection=True;");

            return new AppDbContext(builder.Options);
        }
    }
}