using System.ComponentModel.DataAnnotations;
using StudioX.Application.Editions;
using StudioX.Authorization.Users;
using StudioX.Domain.Entities;
using StudioX.Domain.Entities.Auditing;

namespace StudioX.MultiTenancy
{
    /// <summary>
    /// Represents a Tenant of the application.
    /// </summary>
    public abstract class StudioXTenant<TUser> : StudioXTenantBase, IFullAudited<TUser>
        where TUser : StudioXUserBase
    {
        /// <summary>
        /// "Default".
        /// </summary>
        public const string DefaultTenantName = "Default";

        /// <summary>
        /// "^[a-zA-Z][a-zA-Z0-9_-]{1,}$".
        /// </summary>
        public const string TenancyNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";
        
        /// <summary>
        /// Max length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 128;

        /// <summary>
        /// Current <see cref="Edition"/> of the Tenant.
        /// </summary>
        public virtual Edition Edition { get; set; }
        public virtual int? EditionId { get; set; }

        /// <summary>
        /// Display name of the Tenant.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Reference to the creator user of this entity.
        /// </summary>
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// Reference to the last modifier user of this entity.
        /// </summary>
        public virtual TUser LastModifierUser { get; set; }

        /// <summary>
        /// Reference to the deleter user of this entity.
        /// </summary>
        public virtual TUser DeleterUser { get; set; }

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        protected StudioXTenant()
        {
            IsActive = true;
        }

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        /// <param name="tenancyName">UNIQUE name of this Tenant</param>
        /// <param name="name">Display name of the Tenant</param>
        protected StudioXTenant(string tenancyName, string name)
            : this()
        {
            TenancyName = tenancyName;
            Name = name;
        }
    }
}
