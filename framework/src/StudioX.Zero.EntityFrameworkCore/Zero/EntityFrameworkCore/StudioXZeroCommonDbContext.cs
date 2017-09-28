using StudioX.Auditing;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.EntityFrameworkCore;
using StudioX.Localization;
using StudioX.Notifications;
using StudioX.Organizations;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.EntityFrameworkCore
{
    public abstract class StudioXZeroCommonDbContext<TRole, TUser, TSelf> : StudioXDbContext
        where TRole : StudioXRole<TUser>
        where TUser : StudioXUser<TUser>
        where TSelf: StudioXZeroCommonDbContext<TRole, TUser, TSelf>
    {
        /// <summary>
        /// Roles.
        /// </summary>
        public virtual DbSet<TRole> Roles { get; set; }

        /// <summary>
        /// Users.
        /// </summary>
        public virtual DbSet<TUser> Users { get; set; }

        /// <summary>
        /// User logins.
        /// </summary>
        public virtual DbSet<UserLogin> UserLogins { get; set; }

        /// <summary>
        /// User login attempts.
        /// </summary>
        public virtual DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }

        /// <summary>
        /// User roles.
        /// </summary>
        public virtual DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// User claims.
        /// </summary>
        public virtual DbSet<UserClaim> UserClaims { get; set; }

        /// <summary>
        /// Permissions.
        /// </summary>
        public virtual DbSet<PermissionSetting> Permissions { get; set; }

        /// <summary>
        /// Role permissions.
        /// </summary>
        public virtual DbSet<RolePermissionSetting> RolePermissions { get; set; }

        /// <summary>
        /// User permissions.
        /// </summary>
        public virtual DbSet<UserPermissionSetting> UserPermissions { get; set; }

        /// <summary>
        /// Settings.
        /// </summary>
        public virtual DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Audit logs.
        /// </summary>
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Languages.
        /// </summary>
        public virtual DbSet<ApplicationLanguage> Languages { get; set; }

        /// <summary>
        /// LanguageTexts.
        /// </summary>
        public virtual DbSet<ApplicationLanguageText> LanguageTexts { get; set; }

        /// <summary>
        /// OrganizationUnits.
        /// </summary>
        public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

        /// <summary>
        /// UserOrganizationUnits.
        /// </summary>
        public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }

        /// <summary>
        /// Notifications.
        /// </summary>
        public virtual DbSet<NotificationInfo> Notifications { get; set; }

        /// <summary>
        /// Tenant notifications.
        /// </summary>
        public virtual DbSet<TenantNotificationInfo> TenantNotifications { get; set; }

        /// <summary>
        /// User notifications.
        /// </summary>
        public virtual DbSet<UserNotificationInfo> UserNotifications { get; set; }

        /// <summary>
        /// Notification subscriptions.
        /// </summary>
        public virtual DbSet<NotificationSubscriptionInfo> NotificationSubscriptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected StudioXZeroCommonDbContext(DbContextOptions<TSelf> options)
            :base(options)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TUser>(u =>
            {
                u.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);

                u.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);

                u.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);
            });
        }
    }
}