using System;
using StudioX.Configuration.Startup;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Caching.Configuration;
using StudioX.Runtime.Caching.Redis;
using StudioX.Tests;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;
using Shouldly;

namespace StudioX.RedisCache.Tests
{
    public class RedisCacheManagerTest : TestBaseWithLocalIocManager
    {
        private readonly ITypedCache<string, MyCacheItem> cache;

        public RedisCacheManagerTest()
        {
            LocalIocManager.Register<StudioXRedisCacheOptions>();
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<IStudioXRedisCacheDatabaseProvider, StudioXRedisCacheDatabaseProvider>();
            LocalIocManager.Register<ICacheManager, StudioXRedisCacheManager>();
            LocalIocManager.IocContainer.Register(Component.For<IStudioXStartupConfiguration>().UsingFactoryMethod(() => Substitute.For<IStudioXStartupConfiguration>()));

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyTestCacheItems", cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            cache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, MyCacheItem>("MyTestCacheItems");
            cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        //[Theory]
        //[InlineData("A", 42)]
        //[InlineData("B", 43)]
        public void SimpleGetSetTest(string cacheKey, int cacheValue)
        {
            var item = cache.Get(cacheKey, () => new MyCacheItem { Value = cacheValue });

            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);

            cache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}
