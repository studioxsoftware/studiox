using System.Data.Entity;
using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;

namespace StudioX.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, int>,
        IRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}