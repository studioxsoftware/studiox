using System;
using System.Threading.Tasks;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Caching.Configuration;
using StudioX.Runtime.Caching.Memory;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Runtime.Caching.Memory
{
    public class MemoryCacheManagerTests : TestBaseWithLocalIocManager
    {
        private readonly ICacheManager cacheManager;
        private readonly ITypedCache<string, MyCacheItem> cache;

        public MemoryCacheManagerTests()
        {
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<ICacheManager, StudioXMemoryCacheManager>();
            LocalIocManager.Register<MyClientPropertyInjects>(DependencyLifeStyle.Transient);
            LocalIocManager.IocContainer.Register(Component.For<IStudioXStartupConfiguration>().Instance(Substitute.For<IStudioXStartupConfiguration>()));

            cacheManager = LocalIocManager.Resolve<ICacheManager>();

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            cache = cacheManager.GetCache<string, MyCacheItem>("MyCacheItems");
            cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        [Fact]
        public void SimpleGetSetTest()
        {
            cache.GetOrDefault("A").ShouldBe(null);

            cache.Set("A", new MyCacheItem { Value = 42 });

            cache.GetOrDefault("A").ShouldNotBe(null);
            cache.GetOrDefault("A").Value.ShouldBe(42);

            cache.Get("B", () => new MyCacheItem { Value = 43 }).Value.ShouldBe(43);
            cache.Get("B", () => new MyCacheItem { Value = 44 }).Value.ShouldBe(43); //Does not call factory, so value is not changed
        }

        [Fact]
        public void MultiThreadingTest()
        {
            Parallel.For(
                0,
                2048,
                new ParallelOptions {MaxDegreeOfParallelism = 16},
                i =>
                {
                    var randomKey = RandomHelper.GetRandomOf("A", "B", "C", "D");
                    var randomValue = RandomHelper.GetRandom(0, 16);
                    switch (RandomHelper.GetRandom(0, 3))
                    {
                        case 0:
                            cache.Get(randomKey, () => new MyCacheItem(randomValue));
                            cache.GetOrDefault(randomKey);
                            break;
                        case 1:
                            cache.GetOrDefault(randomKey);
                            cache.Set(randomKey, new MyCacheItem(RandomHelper.GetRandom(0, 16)));
                            cache.GetOrDefault(randomKey);
                            break;
                        case 2:
                            cache.GetOrDefault(randomKey);
                            break;
                    }
                });
        }

        [Fact]
        public void PropertyInjectedCacheManagerShouldWork()
        {
            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(42).ShouldBe(42); //Not in cache, getting from factory
            });

            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(43).ShouldBe(42); //Retrieving from the cache
            });
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }

            public MyCacheItem()
            {
                
            }

            public MyCacheItem(int value)
            {
                Value = value;
            }
        }

        public class MyClientPropertyInjects
        {
            public ICacheManager CacheManager { get; set; }

            public int SetGetValue(int value)
            {
                return CacheManager
                    .GetCache("MyClientPropertyInjectsCache")
                    .Get("A", () =>
                    {
                        return value;
                    });
            }
        }
    }
}
