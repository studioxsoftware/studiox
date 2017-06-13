using StudioX.Dependency;
using StudioX.Runtime.Caching.Configuration;

namespace StudioX.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with MemoryCache.
    /// </summary>
    public class StudioXMemoryCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StudioXMemoryCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterIfNot<StudioXMemoryCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<StudioXMemoryCache>(new { name });
        }
    }
}
