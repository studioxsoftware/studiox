using System.Data.Entity;

namespace StudioX.EntityFramework.Uow
{
    public class ActiveDbContextInfo
    {
        public DbContext DbContext { get; }

        public ActiveDbContextInfo(DbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}