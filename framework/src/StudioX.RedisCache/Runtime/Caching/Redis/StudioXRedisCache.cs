using System;
using System.Reflection;
using StudioX.Domain.Entities;
using StudioX.Reflection.Extensions;
using StackExchange.Redis;

namespace StudioX.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache in a Redis server.
    /// </summary>
    public class StudioXRedisCache : CacheBase
    {
        private readonly IDatabase database;
        private readonly IRedisCacheSerializer serializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StudioXRedisCache(
            string name, 
            IStudioXRedisCacheDatabaseProvider redisCacheDatabaseProvider, 
            IRedisCacheSerializer redisCacheSerializer)
            : base(name)
        {
            database = redisCacheDatabaseProvider.GetDatabase();
            serializer = redisCacheSerializer;
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = database.StringGet(GetLocalizedKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new StudioXException("Can not insert null values to the cache!");
            }

            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently StudioX.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.GetAssembly().FullName.Contains("EntityFrameworkDynamicProxies"))
            {
                type = type.GetTypeInfo().BaseType;
            }

            database.StringSet(
                GetLocalizedKey(key),
                Serialize(value, type),
                absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime
                );
        }

        public override void Remove(string key)
        {
            database.KeyDelete(GetLocalizedKey(key));
        }

        public override void Clear()
        {
            database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }

        protected virtual string Serialize(object value, Type type)
        {
            return serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue objbyte)
        {
            return serializer.Deserialize(objbyte);
        }

        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
