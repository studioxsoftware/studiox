#if !NET46
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

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
            memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
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

            if (absoluteExpireTime != null)
            {
                memoryCache.Set(key, value, DateTimeOffset.Now.Add(absoluteExpireTime.Value));
            }
            else if (slidingExpireTime != null)
            {
                memoryCache.Set(key, value, slidingExpireTime.Value);
            }
            else if (DefaultAbsoluteExpireTime != null)
            {
                memoryCache.Set(key, value, DateTimeOffset.Now.Add(DefaultAbsoluteExpireTime.Value));
            }
            else
            {
                memoryCache.Set(key, value, DefaultSlidingExpireTime);
            }
        }

        public override void Remove(string key)
        {
            memoryCache.Remove(key);
        }

        public override void Clear()
        {
            memoryCache.Dispose();
            memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        public override void Dispose()
        {
            memoryCache.Dispose();
            base.Dispose();
        }
    }
}
#endif