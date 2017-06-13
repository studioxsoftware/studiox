#if NET46
using System;
using System.Runtime.Caching;

namespace StudioX.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICache"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    public class StudioXMemoryCache : CacheBase
    {
        private MemoryCache memoryCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        public StudioXMemoryCache(string name)
            : base(name)
        {
            memoryCache = new MemoryCache(Name);
        }

        public override object GetOrDefault(string key)
        {
            return memoryCache.Get(key);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new StudioXException("Can not insert null values to the cache!");
            }

            var cachePolicy = new CacheItemPolicy();

            if (absoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(absoluteExpireTime.Value);
            }
            else if (slidingExpireTime != null)
            {
                cachePolicy.SlidingExpiration = slidingExpireTime.Value;
            }
            else if(DefaultAbsoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(DefaultAbsoluteExpireTime.Value);
            }
            else
            {
                cachePolicy.SlidingExpiration = DefaultSlidingExpireTime;
            }

            memoryCache.Set(key, value, cachePolicy);
        }

        public override void Remove(string key)
        {
            memoryCache.Remove(key);
        }

        public override void Clear()
        {
            memoryCache.Dispose();
            memoryCache = new MemoryCache(Name);
        }

        public override void Dispose()
        {
            memoryCache.Dispose();
            base.Dispose();
        }
    }
}
#endif