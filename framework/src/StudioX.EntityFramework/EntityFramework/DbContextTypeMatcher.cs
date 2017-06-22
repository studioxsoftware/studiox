using StudioX.Domain.Uow;

namespace StudioX.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<StudioXDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}