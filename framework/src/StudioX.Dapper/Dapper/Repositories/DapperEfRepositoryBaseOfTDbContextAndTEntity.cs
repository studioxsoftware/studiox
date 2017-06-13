using StudioX.Data;
using StudioX.Domain.Entities;
using StudioX.Transactions;

namespace StudioX.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity> : DapperEfRepositoryBase<TDbContext, TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : class

    {
        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(activeTransactionProvider)
        {
        }
    }
}
