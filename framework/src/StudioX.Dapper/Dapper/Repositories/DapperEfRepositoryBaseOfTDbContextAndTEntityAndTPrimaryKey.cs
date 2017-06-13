using System.Data.Common;

using StudioX.Data;
using StudioX.Domain.Entities;

namespace StudioX.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : DapperRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>

    {
        private readonly IActiveTransactionProvider activeTransactionProvider;

        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(activeTransactionProvider)
        {
            this.activeTransactionProvider = activeTransactionProvider;
        }

        public ActiveTransactionProviderArgs ActiveTransactionProviderArgs => new ActiveTransactionProviderArgs
        {
            ["ContextType"] = typeof(TDbContext),
            ["MultiTenancySide"] = MultiTenancySide
        };

        public override DbConnection Connection => (DbConnection)activeTransactionProvider.GetActiveConnection(ActiveTransactionProviderArgs);

        public override DbTransaction ActiveTransaction => (DbTransaction)activeTransactionProvider.GetActiveTransaction(ActiveTransactionProviderArgs);
    }
}
