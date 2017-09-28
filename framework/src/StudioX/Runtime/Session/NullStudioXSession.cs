using StudioX.Configuration.Startup;
using StudioX.MultiTenancy;
using StudioX.Runtime.Remoting;

namespace StudioX.Runtime.Session
{
    /// <summary>
    /// Implements null object pattern for <see cref="IStudioXSession"/>.
    /// </summary>
    public class NullStudioXSession : StudioXSessionBase
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullStudioXSession Instance { get; } = new NullStudioXSession();

        /// <inheritdoc/>
        public override long? UserId => null;

        /// <inheritdoc/>
        public override int? TenantId => null;

        public override MultiTenancySides MultiTenancySide => MultiTenancySides.Tenant;

        public override long? ImpersonatorUserId => null;

        public override int? ImpersonatorTenantId => null;

        private NullStudioXSession() 
            : base(
                  new MultiTenancyConfig(), 
                  new DataContextAmbientScopeProvider<SessionOverride>(new AsyncLocalAmbientDataContext())
            )
        {

        }
    }
}