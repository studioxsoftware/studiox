using StudioX.Domain.Uow;
using StudioX.EntityFramework;

namespace StudioX.EntityFrameworkCore
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<StudioXDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}