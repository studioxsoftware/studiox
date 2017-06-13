using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;
using StudioX.EntityFramework;
using StudioX.EntityFramework.Repositories;

namespace StudioX.TestBase.SampleApplication.EntityFramework.Repositories
{
    public class SampleApplicationEfRepositoryBase<TEntity> : SampleApplicationEfRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    public class SampleApplicationEfRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<SampleApplicationDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}