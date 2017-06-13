using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Configuration
{
    public interface IStudioXDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        void Configure(StudioXDbContextConfiguration<TDbContext> configuration);
    }
}