using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using StudioX.Dapper.Utils;
using StudioX.Domain.Entities;
using StudioX.Domain.Uow;

using DapperExtensions;

namespace StudioX.Dapper.Filters.Query
{
    public class MayHaveTenantDapperQueryFilter : IDapperQueryFilter
    {
        private readonly ICurrentUnitOfWorkProvider currentUnitOfWorkProvider;

        public MayHaveTenantDapperQueryFilter(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            this.currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        private int? TenantId
        {
            get
            {
                DataFilterConfiguration filter = currentUnitOfWorkProvider.Current.Filters.FirstOrDefault(x => x.FilterName == FilterName);
                if (filter.FilterParameters.ContainsKey(StudioXDataFilters.Parameters.TenantId))
                {
                    return (int?)filter.FilterParameters[StudioXDataFilters.Parameters.TenantId];
                }

                return null;
            }
        }

        public string FilterName { get; } = StudioXDataFilters.MayHaveTenant;

        public bool IsEnabled => currentUnitOfWorkProvider.Current.IsFilterEnabled(FilterName);

        public IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            IFieldPredicate predicate = null;
            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)) && IsEnabled)
            {
                predicate = Predicates.Field<TEntity>(f => (f as IMayHaveTenant).TenantId, Operator.Eq, TenantId);
            }
            return predicate;
        }

        public Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)) && IsEnabled)
            {
                PropertyInfo propType = typeof(TEntity).GetProperty(nameof(IMayHaveTenant.TenantId));
                if (predicate == null)
                {
                    predicate = ExpressionUtils.MakePredicate<TEntity>(nameof(IMayHaveTenant.TenantId), TenantId, propType.PropertyType);
                }
                else
                {
                    ParameterExpression paramExpr = predicate.Parameters[0];
                    MemberExpression memberExpr = Expression.Property(paramExpr, nameof(IMayHaveTenant.TenantId));
                    BinaryExpression body = Expression.AndAlso(
                        predicate.Body,
                        Expression.Equal(memberExpr, Expression.Constant(TenantId, propType.PropertyType)));
                    predicate = Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
                }
            }
            return predicate;
        }
    }
}
