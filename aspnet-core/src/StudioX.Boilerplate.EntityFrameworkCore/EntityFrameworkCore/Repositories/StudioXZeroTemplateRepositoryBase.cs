using StudioX.Domain.Entities;
using StudioX.EntityFrameworkCore;
using StudioX.EntityFrameworkCore.Repositories;

namespace StudioX.Boilerplate.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class StudioXZeroTemplateRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<BoilerplateDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected StudioXZeroTemplateRepositoryBase(IDbContextProvider<BoilerplateDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add your common methods for all repositories
    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="StudioXZeroTemplateRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class StudioXZeroTemplateRepositoryBase<TEntity> : StudioXZeroTemplateRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected StudioXZeroTemplateRepositoryBase(IDbContextProvider<BoilerplateDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)!!!
    }
}
