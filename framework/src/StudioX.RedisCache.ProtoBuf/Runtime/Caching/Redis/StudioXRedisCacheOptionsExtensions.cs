using StudioX.Configuration.Startup;
using StudioX.Dependency;

namespace StudioX.Runtime.Caching.Redis
{
    public static class StudioXRedisCacheOptionsExtensions
    {
        public static void UseProtoBuf(this StudioXRedisCacheOptions options)
        {
            options.StudioXStartupConfiguration
                .ReplaceService<IRedisCacheSerializer, ProtoBufRedisCacheSerializer>(DependencyLifeStyle.Transient);
        }
    }
}
