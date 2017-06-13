namespace StudioX.MultiTenancy
{
    public interface ITenantResolveContributor
    {
        int? ResolveTenantId();
    }
}