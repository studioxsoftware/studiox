using StudioX.Authorization.Users;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Security;

namespace StudioX.MultiTenancy
{
    public class TenantCache<TTenant, TUser> : ITenantCache, IEventHandler<EntityChangedEventData<TTenant>>
        where TTenant : StudioXTenant<TUser>
        where TUser : StudioXUserBase
    {
        private readonly ICacheManager cacheManager;
        private readonly IRepository<TTenant> tenantRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public TenantCache(
            ICacheManager cacheManager,
            IRepository<TTenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.cacheManager = cacheManager;
            this.tenantRepository = tenantRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public virtual TenantCacheItem Get(int tenantId)
        {
            var cacheItem = GetOrNull(tenantId);

            if (cacheItem == null)
            {
                throw new StudioXException("There is no tenant with given id: " + tenantId);
            }

            return cacheItem;
        }

        public virtual TenantCacheItem Get(string tenancyName)
        {
            var cacheItem = GetOrNull(tenancyName);

            if (cacheItem == null)
            {
                throw new StudioXException("There is no tenant with given tenancy name: " + tenancyName);
            }

            return cacheItem;
        }

        public virtual TenantCacheItem GetOrNull(string tenancyName)
        {
            var tenantId = cacheManager
                .GetTenantByNameCache()
                .Get(
                    tenancyName.ToLowerInvariant(),
                    () => GetTenantOrNull(tenancyName)?.Id
                );

            if (tenantId == null)
            {
                return null;
            }

            return Get(tenantId.Value);
        }

        public TenantCacheItem GetOrNull(int tenantId)
        {
            return cacheManager
                .GetTenantCache()
                .Get(
                    tenantId,
                    () =>
                    {
                        var tenant = GetTenantOrNull(tenantId);
                        if (tenant == null)
                        {
                            return null;
                        }

                        return CreateTenantCacheItem(tenant);
                    }
                );
        }

        protected virtual TenantCacheItem CreateTenantCacheItem(TTenant tenant)
        {
            return new TenantCacheItem
            {
                Id = tenant.Id,
                Name = tenant.Name,
                TenancyName = tenant.TenancyName,
                EditionId = tenant.EditionId,
                ConnectionString = SimpleStringCipher.Instance.Decrypt(tenant.ConnectionString),
                IsActive = tenant.IsActive
            };
        }

        [UnitOfWork]
        protected virtual TTenant GetTenantOrNull(int tenantId)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                return tenantRepository.FirstOrDefault(tenantId);
            }
        }

        [UnitOfWork]
        protected virtual TTenant GetTenantOrNull(string tenancyName)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                return tenantRepository.FirstOrDefault(t => t.TenancyName == tenancyName);
            }
        }

        public void HandleEvent(EntityChangedEventData<TTenant> eventData)
        {
            var existingCacheItem = cacheManager.GetTenantCache().GetOrDefault(eventData.Entity.Id);

            cacheManager
                .GetTenantByNameCache()
                .Remove(
                    existingCacheItem != null
                        ? existingCacheItem.TenancyName.ToLowerInvariant()
                        : eventData.Entity.TenancyName.ToLowerInvariant()
                );

            cacheManager
                .GetTenantCache()
                .Remove(eventData.Entity.Id);
        }
    }
}