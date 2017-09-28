using StudioX.Domain.Uow;

namespace StudioX.MultiTenancy
{
    /// <summary>
    /// Extends <see cref="IConnectionStringResolver"/> to
    /// get connection string for given tenant.
    /// </summary>
    public interface IDbPerTenantConnectionStringResolver : IConnectionStringResolver
    {
        /// <summary>
        /// Gets the connection string for given args.
        /// </summary>
        string GetNameOrConnectionString(DbPerTenantConnectionStringResolveArgs args);
    }
}
