using System;
using System.Linq.Expressions;
using RefactorThis.GraphDiff;

namespace StudioX.EntityFramework.GraphDiff.Mapping
{
    public interface IEntityMappingManager
    {
        Expression<Func<IUpdateConfiguration<TEntity>, object>> GetEntityMappingOrNull<TEntity>();
    }
}