using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using StudioX.Application.Services;
using StudioX.Domain.Repositories;

namespace StudioX.Domain.Uow
{
    internal class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        public TransactionScopeOption Scope { get; set; }

        /// <inheritdoc/>
        public bool IsTransactional { get; set; }

        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }

#if NET46
        /// <inheritdoc/>
        public bool IsTransactionScopeAvailable { get; set; }
#endif

        /// <inheritdoc/>
        public IsolationLevel? IsolationLevel { get; set; }

        public IReadOnlyList<DataFilterConfiguration> Filters => filters;
        private readonly List<DataFilterConfiguration> filters;

        public List<Func<Type, bool>> ConventionalUowSelectors { get; }

        public UnitOfWorkDefaultOptions()
        {
            filters = new List<DataFilterConfiguration>();
            IsTransactional = true;
            Scope = TransactionScopeOption.Required;

#if NET46
            IsTransactionScopeAvailable = true;
#endif

            ConventionalUowSelectors = new List<Func<Type, bool>>
            {
                type => typeof(IRepository).IsAssignableFrom(type) ||
                        typeof(IApplicationService).IsAssignableFrom(type)
            };
        }

        public void RegisterFilter(string filterName, bool isEnabledByDefault)
        {
            if (filters.Any(f => f.FilterName == filterName))
            {
                throw new StudioXException("There is already a filter with name: " + filterName);
            }

            filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }

        public void OverrideFilter(string filterName, bool isEnabledByDefault)
        {
            filters.RemoveAll(f => f.FilterName == filterName);
            filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }
    }
}