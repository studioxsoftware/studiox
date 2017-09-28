using Microsoft.EntityFrameworkCore;

namespace StudioX.IdentityServer4
{
    public interface IStudioXPersistedGrantDbContext
    {
        DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
    }
}