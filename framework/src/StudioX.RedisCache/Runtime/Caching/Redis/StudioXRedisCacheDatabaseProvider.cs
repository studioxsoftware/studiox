using System;
using StudioX.Dependency;
using StackExchange.Redis;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// Implements <see cref="IStudioXRedisCacheDatabaseProvider"/>.
    /// </summary>
    public class StudioXRedisCacheDatabaseProvider : IStudioXRedisCacheDatabaseProvider, ISingletonDependency
    {
        private readonly StudioXRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public StudioXRedisCacheDatabaseProvider(StudioXRedisCacheOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
    }
}
