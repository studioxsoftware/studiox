using StudioX.Dependency;
using StudioX.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Uow
{
  public interface IEfCoreTransactionStrategy
  {
    void InitOptions(UnitOfWorkOptions options);

    DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
        where TDbContext : DbContext;

    void Commit();

    void Dispose(IIocResolver iocResolver);
  }
}
