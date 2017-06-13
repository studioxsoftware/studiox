using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Auditing;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.BackgroundJobs;
using StudioX.Configuration;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Notifications;
using StudioX.Organizations;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for <see cref="DbModelBuilder"/>.
    /// </summary>
    public static class StudioXZeroDbModelBuilderExtensions
    {
        /// <summary>
        /// Changes prefix for StudioX tables (which is "StudioX" by default).
        /// Can be null/empty string to clear the prefix.
        /// </summary>
        /// <typeparam name="TTenant">The type of the tenant entity.</typeparam>
        /// <typeparam name="TRole">The type of the role entity.</typeparam>
        /// <typeparam name="TUser">The type of the user entity.</typeparam>
        /// <param name="modelBuilder">Model builder.</param>
        /// <param name="prefix">Table prefix, or null to clear prefix.</param>
        public static void ChangeStudioXTablePrefix<TTenant, TRole, TUser>(this ModelBuilder modelBuilder, string prefix, string schemaName = null)
            where TTenant : StudioXTenant<TUser>
            where TRole : StudioXRole<TUser>
            where TUser : StudioXUser<TUser>
        {
            prefix = prefix ?? "";

            SetTableName<AuditLog>(modelBuilder, prefix + "AuditLogs", schemaName);
            SetTableName<BackgroundJobInfo>(modelBuilder, prefix + "BackgroundJobs", schemaName);
            SetTableName<Edition>(modelBuilder, prefix + "Editions", schemaName);
            SetTableName<FeatureSetting>(modelBuilder, prefix + "Features", schemaName);
            SetTableName<TenantFeatureSetting>(modelBuilder, prefix + "Features", schemaName);
            SetTableName<EditionFeatureSetting>(modelBuilder, prefix + "Features", schemaName);
            SetTableName<ApplicationLanguage>(modelBuilder, prefix + "Languages", schemaName);
            SetTableName<ApplicationLanguageText>(modelBuilder, prefix + "LanguageTexts", schemaName);
            SetTableName<NotificationInfo>(modelBuilder, prefix + "Notifications", schemaName);
            SetTableName<NotificationSubscriptionInfo>(modelBuilder, prefix + "NotificationSubscriptions", schemaName);
            SetTableName<OrganizationUnit>(modelBuilder, prefix + "OrganizationUnits", schemaName);
            SetTableName<PermissionSetting>(modelBuilder, prefix + "Permissions", schemaName);
            SetTableName<RolePermissionSetting>(modelBuilder, prefix + "Permissions", schemaName);
            SetTableName<UserPermissionSetting>(modelBuilder, prefix + "Permissions", schemaName);
            SetTableName<TRole>(modelBuilder, prefix + "Roles", schemaName);
            SetTableName<Setting>(modelBuilder, prefix + "Settings", schemaName);
            SetTableName<TTenant>(modelBuilder, prefix + "Tenants", schemaName);
            SetTableName<UserLogin>(modelBuilder, prefix + "UserLogins", schemaName);
            SetTableName<UserLoginAttempt>(modelBuilder, prefix + "UserLoginAttempts", schemaName);
            SetTableName<TenantNotificationInfo>(modelBuilder, prefix + "TenantNotifications", schemaName);
            SetTableName<UserNotificationInfo>(modelBuilder, prefix + "UserNotifications", schemaName);
            SetTableName<UserOrganizationUnit>(modelBuilder, prefix + "UserOrganizationUnits", schemaName);
            SetTableName<UserRole>(modelBuilder, prefix + "UserRoles", schemaName);
            SetTableName<TUser>(modelBuilder, prefix + "Users", schemaName);
            SetTableName<UserAccount>(modelBuilder, prefix + "UserAccounts", schemaName);
            SetTableName<UserClaim>(modelBuilder, prefix + "UserClaims", schemaName);
        }

        private static void SetTableName<TEntity>(ModelBuilder modelBuilder, string tableName, string schemaName)
            where TEntity : class
        {
            if (schemaName == null)
            {
                modelBuilder.Entity<TEntity>().ToTable(tableName);
            }
            else
            {
                modelBuilder.Entity<TEntity>().ToTable(tableName, schemaName);                
            }
        }
    }
}