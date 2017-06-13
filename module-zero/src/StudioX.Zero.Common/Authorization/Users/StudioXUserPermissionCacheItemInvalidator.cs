using StudioX.Dependency;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;

namespace StudioX.Authorization.Users
{
    public class StudioXUserPermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityDeletedEventData<StudioXUserBase>>,

        ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public StudioXUserPermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? 0);
            cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? 0);
            cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<StudioXUserBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? 0);
            cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }
    }
}