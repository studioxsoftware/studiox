using Microsoft.EntityFrameworkCore;

namespace StudioX.Boilerplate.EntityFrameworkCore
{
    public static class BoilerplateDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<BoilerplateDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}