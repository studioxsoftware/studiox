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
        private readonly StudioXRedisCacheOptions options;
        private readonly Lazy<ConnectionMultiplexer> connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public StudioXRedisCacheDatabaseProvider(StudioXRedisCacheOptions options)
        {
            this.options = options;
            connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return connectionMultiplexer.Value.GetDatabase(options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(options.ConnectionString);
        }
    }
}
