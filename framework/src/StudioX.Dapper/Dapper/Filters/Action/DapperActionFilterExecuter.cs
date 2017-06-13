using StudioX.Dependency;
using StudioX.Domain.Entities;

namespace StudioX.Dapper.Filters.Action
{
    public class DapperActionFilterExecuter : IDapperActionFilterExecuter, ITransientDependency
    {
        private readonly IIocResolver iocResolver;

        public DapperActionFilterExecuter(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public void ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            iocResolver.Resolve<CreationAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            iocResolver.Resolve<ModificationAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            iocResolver.Resolve<DeletionAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }
    }
}
