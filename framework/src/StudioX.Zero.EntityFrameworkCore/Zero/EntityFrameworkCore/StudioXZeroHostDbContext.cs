using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.BackgroundJobs;
using StudioX.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.EntityFrameworkCore
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class StudioXZeroHostDbContext<TTenant, TRole, TUser, TSelf> : StudioXZeroCommonDbContext<TRole, TUser, TSelf>
        where TTenant : StudioXTenant<TUser>
        where TRole : StudioXRole<TUser>
        where TUser : StudioXUser<TUser>
        where TSelf : StudioXZeroHostDbContext<TTenant, TRole, TUser, TSelf>
    {
        /// <summary>
        /// Tenants
        /// </summary>
        public virtual DbSet<TTenant> Tenants { get; set; }

        /// <summary>
        /// Editions.
        /// </summary>
        public virtual DbSet<Edition> Editions { get; set; }

        /// <summary>
        /// FeatureSettings.
        /// </summary>
        public virtual DbSet<FeatureSetting> FeatureSettings { get; set; }

        /// <summary>
        /// TenantFeatureSetting.
        /// </summary>
        public virtual DbSet<TenantFeatureSetting> TenantFeatureSettings { get; set; }

        /// <summary>
        /// EditionFeatureSettings.
        /// </summary>
        public virtual DbSet<EditionFeatureSetting> EditionFeatureSettings { get; set; }

        /// <summary>
        /// Background jobs.
        /// </summary>
        public virtual DbSet<BackgroundJobInfo> BackgroundJobs { get; set; }

        /// <summary>
        /// User accounts
        /// </summary>
        public virtual DbSet<UserAccount> UserAccounts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected StudioXZeroHostDbContext(DbContextOptions<TSelf> options)
            :base(options)
        {

        }
    }
}