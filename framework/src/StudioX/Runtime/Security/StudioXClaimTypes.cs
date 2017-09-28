using System.Security.Claims;

namespace StudioX.Runtime.Security
{
    /// <summary>
    /// Used to get StudioX-specific claim type names.
    /// </summary>
    public static class StudioXClaimTypes
    {
        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.NameIdentifier"/>
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.NameIdentifier"/>
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.NameIdentifier"/>
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;

        /// <summary>
        /// TenantId.
        /// </summary>
        public static string TenantId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/tenantId";

        /// <summary>
        /// ImpersonatorUserId.
        /// </summary>
        public static string ImpersonatorUserId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/impersonatorUserId";

        /// <summary>
        /// ImpersonatorTenantId
        /// </summary>
        public static string ImpersonatorTenantId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/impersonatorTenantId";
    }
}
