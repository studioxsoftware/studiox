using Microsoft.EntityFrameworkCore;
using StudioX.MultiTenancy;

namespace StudioX.EntityFrameworkCore
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();

        TDbContext GetDbContext(MultiTenancySides? multiTenancySide);
    }
}