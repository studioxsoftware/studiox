namespace StudioX.MultiTenancy
{
    public class NullTenantStore : ITenantStore
    {
        public TenantInfo Find(int tenantId)
        {
            return null;
        }

        public TenantInfo Find(string tenancyName)
        {
            return null;
        }
    }
}