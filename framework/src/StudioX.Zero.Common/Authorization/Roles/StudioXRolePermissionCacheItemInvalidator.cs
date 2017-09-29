using StudioX.Dependency;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;

namespace StudioX.Authorization.Roles
{
    public class StudioXRolePermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<RolePermissionSetting>>,
        IEventHandler<EntityDeletedEventData<StudioXRoleBase>>,
        ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public StudioXRolePermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<RolePermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.TenantId ?? 0);
            cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<StudioXRoleBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? 0);
            cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }
    }
}