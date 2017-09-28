using System.Collections.Generic;
using StudioX.Authorization.Users;
using StudioX.Threading;

namespace StudioX.MultiTenancy
{
    public static class StudioXTenantManagerExtensions
    {
        public static void Create<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, TTenant tenant)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.CreateAsync(tenant));
        }

        public static void Update<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, TTenant tenant)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.UpdateAsync(tenant));
        }

        public static TTenant FindById<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int id)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            return AsyncHelper.RunSync(() => tenantManager.FindByIdAsync(id));
        }

        public static TTenant GetById<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int id)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            return AsyncHelper.RunSync(() => tenantManager.GetByIdAsync(id));
        }

        public static TTenant FindByTenancyName<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, string tenancyName)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            return AsyncHelper.RunSync(() => tenantManager.FindByTenancyNameAsync(tenancyName));
        }

        public static void Delete<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, TTenant tenant)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.DeleteAsync(tenant));
        }

        public static string GetFeatureValueOrNull<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int tenantId, string featureName)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            return AsyncHelper.RunSync(() => tenantManager.GetFeatureValueOrNullAsync(tenantId, featureName));
        }

        public static IReadOnlyList<NameValue> GetFeatureValues<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int tenantId)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            return AsyncHelper.RunSync(() => tenantManager.GetFeatureValuesAsync(tenantId));
        }

        public static void SetFeatureValues<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int tenantId, params NameValue[] values)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.SetFeatureValuesAsync(tenantId, values));
        }

        public static void SetFeatureValue<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int tenantId, string featureName, string value)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.SetFeatureValueAsync(tenantId, featureName, value));
        }

        public static void SetFeatureValue<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, TTenant tenant, string featureName, string value)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.SetFeatureValueAsync(tenant, featureName, value));
        }

        public static void ResetAllFeatures<TTenant, TUser>(this StudioXTenantManager<TTenant, TUser> tenantManager, int tenantId)
            where TTenant : StudioXTenant<TUser>
            where TUser : StudioXUserBase
        {
            AsyncHelper.RunSync(() => tenantManager.ResetAllFeaturesAsync(tenantId));
        }

    }
}