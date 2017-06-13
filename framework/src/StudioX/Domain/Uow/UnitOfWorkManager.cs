using System.Linq;
using System.Transactions;
using StudioX.Dependency;

namespace StudioX.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly ICurrentUnitOfWorkProvider currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions defaultOptions;

        public IActiveUnitOfWork Current => currentUnitOfWorkProvider.Current;

        public UnitOfWorkManager(
            IIocResolver iocResolver,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            this.iocResolver = iocResolver;
            this.currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            this.defaultOptions = defaultOptions;
        }

        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(defaultOptions);

            var outerUow = currentUnitOfWorkProvider.Current;

            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                return new InnerUnitOfWorkCompleteHandle();
            }

            var uow = iocResolver.Resolve<IUnitOfWork>();

            uow.Completed += (sender, args) =>
            {
                currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                iocResolver.Release(uow);
            };

            //Inherit filters from outer UOW
            if (outerUow != null)
            {
                options.FillOuterUowFiltersForNonProvidedOptions(outerUow.Filters.ToList());
            }

            uow.Begin(options);

            //Inherit tenant from outer UOW
            if (outerUow != null)
            {
                uow.SetTenantId(outerUow.GetTenantId(), false);
            }

            currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}