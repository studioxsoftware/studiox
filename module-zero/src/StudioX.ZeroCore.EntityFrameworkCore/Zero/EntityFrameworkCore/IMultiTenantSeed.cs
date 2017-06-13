using StudioX.MultiTenancy;

namespace StudioX.Zero.EntityFrameworkCore
{
    public interface IMultiTenantSeed
    {
        StudioXTenantBase Tenant { get; set; }
    }
}