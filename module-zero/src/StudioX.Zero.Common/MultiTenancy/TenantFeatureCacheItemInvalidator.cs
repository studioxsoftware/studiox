using StudioX.Dependency;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;

namespace StudioX.MultiTenancy
{
    /// <summary>
    /// This class handles related events and invalidated tenant feature cache items if needed.
    /// </summary>
    public class TenantFeatureCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<TenantFeatureSetting>>,
        ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureCacheItemInvalidator"/> class.
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        public TenantFeatureCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<TenantFeatureSetting> eventData)
        {
            cacheManager.GetTenantFeatureCache().Remove(eventData.Entity.TenantId);
        }
    }
}