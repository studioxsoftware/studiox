using System;
using System.Linq.Expressions;

using StudioX.Dependency;
using StudioX.Domain.Entities;

using DapperExtensions;

namespace StudioX.Dapper.Filters.Query
{
    public interface IDapperQueryFilter : ITransientDependency
    {
        string FilterName { get; }

        bool IsEnabled { get; }

        IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;

        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
