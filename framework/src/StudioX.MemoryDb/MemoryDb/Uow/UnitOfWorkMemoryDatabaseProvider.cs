using StudioX.Dependency;
using StudioX.Domain.Uow;

namespace StudioX.MemoryDb.Uow
{
    /// <summary>
    /// Implements <see cref="IMemoryDatabaseProvider"/> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMemoryDatabaseProvider : IMemoryDatabaseProvider, ITransientDependency
    {
        public MemoryDatabase Database => ((MemoryDbUnitOfWork)currentUnitOfWork.Current).Database;

        private readonly ICurrentUnitOfWorkProvider currentUnitOfWork;

        public UnitOfWorkMemoryDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            this.currentUnitOfWork = currentUnitOfWork;
        }
    }
}