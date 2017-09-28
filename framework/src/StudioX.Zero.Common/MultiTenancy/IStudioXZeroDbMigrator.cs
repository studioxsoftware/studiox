namespace StudioX.MultiTenancy
{
    public interface IStudioXZeroDbMigrator
    {
        void CreateOrMigrateForHost();

        void CreateOrMigrateForTenant(StudioXTenantBase tenant);
    }
}
