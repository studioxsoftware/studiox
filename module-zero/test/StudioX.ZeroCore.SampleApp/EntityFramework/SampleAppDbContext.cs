using StudioX.IdentityServer4;
using StudioX.Zero.EntityFrameworkCore;
using StudioX.ZeroCore.SampleApp.Core;
using Microsoft.EntityFrameworkCore;

namespace StudioX.ZeroCore.SampleApp.EntityFramework
{
    public class SampleAppDbContext : StudioXZeroDbContext<Tenant, Role, User, SampleAppDbContext>, IStudioXPersistedGrantDbContext
    {
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
