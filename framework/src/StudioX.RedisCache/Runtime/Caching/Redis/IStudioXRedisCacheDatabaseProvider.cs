using StackExchange.Redis;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to get <see cref="IDatabase"/> for Redis cache.
    /// </summary>
    public interface IStudioXRedisCacheDatabaseProvider 
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
