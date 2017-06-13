using StudioX.Dependency;
using StudioX.Runtime.Caching.Configuration;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="StudioXRedisCache"/> instances.
    /// </summary>
    public class StudioXRedisCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXRedisCacheManager"/> class.
        /// </summary>
        public StudioXRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterIfNot<StudioXRedisCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<StudioXRedisCache>(new { name });
        }
    }
}
