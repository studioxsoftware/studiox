using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore;
using StudioX.MultiTenancy;
using StudioX.Zero.EntityFrameworkCore;

namespace StudioX.Boilerplate.EntityFrameworkCore
{
    public class StudioXZeroDbMigrator : StudioXZeroDbMigrator<BoilerplateDbContext>
    {
        public StudioXZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver) :
            base(
                unitOfWorkManager,
                connectionStringResolver,
                dbContextResolver)
        {

        }
    }
}
