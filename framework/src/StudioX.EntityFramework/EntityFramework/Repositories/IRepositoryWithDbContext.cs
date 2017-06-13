using System.Data.Entity;

namespace StudioX.EntityFramework.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}