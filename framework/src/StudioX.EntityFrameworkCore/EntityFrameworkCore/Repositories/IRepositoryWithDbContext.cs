using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}