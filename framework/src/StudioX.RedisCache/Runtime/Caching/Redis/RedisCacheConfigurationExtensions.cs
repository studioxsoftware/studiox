using System;
using StudioX.Dependency;
using StudioX.Runtime.Caching.Configuration;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseRedis(options => { });
        }

        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<StudioXRedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.StudioXConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, StudioXRedisCacheManager>();

            optionsAction(iocManager.Resolve<StudioXRedisCacheOptions>());
        }
    }
}
