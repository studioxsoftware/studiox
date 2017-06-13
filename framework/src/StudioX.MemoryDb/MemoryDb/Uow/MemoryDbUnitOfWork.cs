using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.MemoryDb.Configuration;

namespace StudioX.MemoryDb.Uow
{
    /// <summary>
    /// Implements Unit of work for MemoryDb.
    /// </summary>
    public class MemoryDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        /// <summary>
        /// Gets a reference to Memory Database.
        /// </summary>
        public MemoryDatabase Database { get; private set; }

        private readonly IStudioXMemoryDbModuleConfiguration configuration;
        private readonly MemoryDatabase memoryDatabase;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryDbUnitOfWork(
            IStudioXMemoryDbModuleConfiguration configuration, 
            MemoryDatabase memoryDatabase, 
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(
                  connectionStringResolver, 
                  defaultOptions,
                  filterExecuter)
        {
            this.configuration = configuration;
            this.memoryDatabase = memoryDatabase;
        }

        protected override void BeginUow()
        {
            Database = memoryDatabase;
        }

        public override void SaveChanges()
        {

        }

        #pragma warning disable 1998
        public override async Task SaveChangesAsync()
        {

        }
        #pragma warning restore 1998

        protected override void CompleteUow()
        {

        }

        #pragma warning disable 1998
        protected override async Task CompleteUowAsync()
        {

        }
        #pragma warning restore 1998

        protected override void DisposeUow()
        {

        }
    }
}