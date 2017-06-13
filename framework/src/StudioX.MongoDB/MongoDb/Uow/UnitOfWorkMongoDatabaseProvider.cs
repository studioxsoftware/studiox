using StudioX.Dependency;
using StudioX.Domain.Uow;
using MongoDB.Driver;

namespace StudioX.MongoDb.Uow
{
    /// <summary>
    /// Implements <see cref="IMongoDatabaseProvider"/> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMongoDatabaseProvider : IMongoDatabaseProvider, ITransientDependency
    {
        public MongoDatabase Database => ((MongoDbUnitOfWork)currentUnitOfWork.Current).Database;

        private readonly ICurrentUnitOfWorkProvider currentUnitOfWork;

        public UnitOfWorkMongoDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            this.currentUnitOfWork = currentUnitOfWork;
        }
    }
}