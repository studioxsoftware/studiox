using StudioX.MultiTenancy;

namespace StudioX.Zero.EntityFramework
{
    public interface IMultiTenantSeed
    {
        StudioXTenantBase Tenant { get; set; }
    }
}