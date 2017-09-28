using System.IdentityModel.Tokens.Jwt;
using StudioX.Runtime.Security;

namespace StudioX.IdentityServer4
{
    public class StudioXIdentityServerOptions
    {
        /// <summary>
        /// Updates <see cref="JwtSecurityTokenHandler.DefaultInboundClaimTypeMap"/> to be compatible with identity server claims.
        /// Default: true.
        /// </summary>
        public bool UpdateJwtSecurityTokenHandlerDefaultInboundClaimTypeMap { get; set; } = true;

        /// <summary>
        /// Updates <see cref="StudioXClaimTypes"/> to be compatible with identity server claims.
        /// Default: true.
        /// </summary>
        public bool UpdateStudioXClaimTypes { get; set; } = true;
    }
}