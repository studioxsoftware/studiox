namespace StudioX.MultiTenancy
{
    public interface ITenantResolver
    {
        int? ResolveTenantId();
    }
}