using Microsoft.EntityFrameworkCore;

namespace StudioX.ZeroCore.SampleApp.EntityFramework
{
    public static class StudioXZeroTemplateDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SampleAppDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}