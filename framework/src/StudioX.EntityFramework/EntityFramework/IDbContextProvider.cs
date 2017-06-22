using System.Data.Entity;
using StudioX.MultiTenancy;

namespace StudioX.EntityFramework
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();

        TDbContext GetDbContext(MultiTenancySides? multiTenancySide);
    }
}